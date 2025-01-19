using Vrap.Database;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration["Vrap:PostgresConnectionString"];

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("Vrap:PostgresConnectionString not defined");
}

builder.Services.AddNpgsql<VrapDbContext>(
	connectionString,
	options => options.MigrationsAssembly(typeof(VrapDbContext).Assembly));

var host = builder.Build();

host.Services.GetRequiredService<ILogger<Program>>().LogWarning(
"""
Migrating database? Added a hypertable?
Remember to update the generated migration file with migrationBuilder.CreateHyperTable
""");

host.Run();
