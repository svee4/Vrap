using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Vrap.LifeLog.Web.Database;
using Vrap.LifeLog.Web.Infra;
using Vrap.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.AddVrapDatabase(builder.Configuration.GetRequiredConfiguration("Vrap:PostgresConnectionString"));

builder.Services.AddNpgsql<LifeLogDbContext>(builder.Configuration.GetRequiredConfiguration("Vrap:LifeLog:PostgresConnectionString"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<LifeLogDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Auth/Login";
	options.AccessDeniedPath = "/Auth/Login";
});

builder.Services.AddAuthentication()
	.AddMicrosoftAccount(options =>
	{
		options.Events.OnRedirectToAuthorizationEndpoint += (RedirectContext<OAuthOptions> context) =>
		{
			context.HttpContext.Response.Redirect(context.RedirectUri + "&prompt=select_account");
			return Task.CompletedTask;
		};

		options.ClientId = builder.Configuration.GetRequiredConfiguration("Vrap:LifeLog:Auth:Microsoft:ClientId");
		options.ClientSecret = builder.Configuration.GetRequiredConfiguration("Vrap:LifeLog:Auth:Microsoft:ClientSecret");
	});

builder.Services.AddAuthorizationBuilder()
	.SetFallbackPolicy(new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build());


builder.Services.AddScoped<HumanizerService>();

builder.Services.AddControllersWithViews(options => options.ModelBinderProviders.Insert(0,
		new Vrap.LifeLog.Web.Features.DataTables.Table.Edit.EditController.AddFieldModelBinderProvider()));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	_ = app.UseForwardedHeaders();
	_ = app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
	logger.LogInformation("Migrating database");
	await using var dbContext = scope.ServiceProvider.GetRequiredService<LifeLogDbContext>();
	await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
