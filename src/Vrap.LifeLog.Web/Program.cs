using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Vrap.LifeLog.Web.Database;
using Vrap.LifeLog.Web.Infra;
using Vrap.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.AddVrapDatabase(builder.Configuration.GetRequiredConfiguration("Vrap:PostgresConnectionString"));

builder.Services.AddNpgsql<LifeLogDbContext>(builder.Configuration.GetRequiredConfiguration("Vrap:LifeLog:PostgresConnectionString"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>();

builder.Services.AddAuthorizationBuilder()
	.SetFallbackPolicy(new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build());


//builder.

builder.Services.AddScoped<HumanizerService>();

builder.Services.AddControllersWithViews(options => options.ModelBinderProviders.Insert(0,
		new Vrap.LifeLog.Web.Features.DataTables.Table.Edit.EditController.AddFieldModelBinderProvider()));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	_ = app.UseExceptionHandler("/Home/Error");
	_ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.Run();
