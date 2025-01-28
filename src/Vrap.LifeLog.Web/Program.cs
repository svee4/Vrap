using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vrap.LifeLog.Web.Database;
using Vrap.LifeLog.Web.Infra;
using Vrap.LifeLog.Web.Infra.RequestServices;
using Vrap.Shared;

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RequestFeatures>();
builder.Services.AddScoped<RequestCulture>();
builder.Services.AddScoped<RequestTimeZone>();
builder.Services.AddScoped<HumanizerService>();

builder.Services.AddControllersWithViews(options => options.ModelBinderProviders.Insert(0,
		new Vrap.LifeLog.Web.Features.DataTables.Table.Edit.EditController.AddFieldModelBinderProvider()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	var options = new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	};

	// this is safe because this app in production is only accessible via the proxy
	options.KnownNetworks.Clear();
	options.KnownProxies.Clear();

	_ = app.UseForwardedHeaders(options);
	_ = app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseTimeZoneMiddleware();

app.MapStaticAssets();
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
	scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogInformation("Migrating database");
	await using var dbContext = scope.ServiceProvider.GetRequiredService<LifeLogDbContext>();
	await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
