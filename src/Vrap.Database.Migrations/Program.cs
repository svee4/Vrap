using Vrap.Database;

//System.Diagnostics.Debugger.Launch();

var builder = Host.CreateApplicationBuilder(args);

const string Message = """
Migrating database? Added a hypertable?
Remember to update the generated migration file with migrationBuilder.CreateHyperTable
""";

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogWarning(Message);

string connectionString;
if (args.Contains("--no-connection"))
{
	logger.LogInformation("Skipping connection string");
	connectionString = "";
}
else
{
	connectionString = builder.Configuration["Vrap:PostgresConnectionString"]
		?? throw new InvalidOperationException("Vrap:PostgresConnectionString not defined");
}

builder.Services.AddNpgsql<VrapDbContext>(
	connectionString,
	options => options.MigrationsAssembly(typeof(VrapDbContext).Assembly));

var host = builder.Build();

host.Run();
