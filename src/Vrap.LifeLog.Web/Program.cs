using Vrap.LifeLog.Web.Infra;
using Vrap.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.AddVrapDatabase(builder.Configuration.GetRequiredConfiguration("Vrap:PostgresConnectionString"));

builder.Services.AddScoped<HumanizerService>();

builder.Services.AddControllersWithViews(options =>
{
	options.ModelBinderProviders.Insert(0,
		new Vrap.LifeLog.Web.Features.DataTables.Table.Edit.EditController.AddFieldModelBinderProvider());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.Run();
