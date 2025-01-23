using Vrap.Database;

// System.Diagnostics.Debugger.Launch();

var builder = Host.CreateApplicationBuilder(args);

const string Message = """
Migrating database? Added a hypertable?
Remember to update the generated migration file with migrationBuilder.CreateHyperTable
""";

builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>().LogWarning(Message);

var connectionString = builder.Configuration["Vrap:PostgresConnectionString"];

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("Vrap:PostgresConnectionString not defined");
}

builder.Services.AddNpgsql<VrapDbContext>(
	connectionString,
	options => options.MigrationsAssembly(typeof(VrapDbContext).Assembly));

var host = builder.Build();


host.Run();
