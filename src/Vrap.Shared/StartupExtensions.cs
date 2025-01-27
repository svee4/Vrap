using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Serilog;
using System.Globalization;
using Vrap.Database;

namespace Vrap.Shared;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style",
	"IDE0058:Expression value is never used",
	Justification = "Too much noise")]
public static class StartupExtensions
{
	public static void AddSerilog(this IHostApplicationBuilder builder) =>
		builder.AddSerilog((_, _) => { });

	public static void AddSerilog(
		this IHostApplicationBuilder builder,
		Action<IServiceProvider, LoggerConfiguration> configure)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.Services.AddSerilog((sp, config) =>
		{
			config
				.Enrich.FromLogContext()
				.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>());

			configure(sp, config);
		});
	}

	public static void AddVrapDatabase(
		this IHostApplicationBuilder builder,
		string connectionString,
		Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null,
		Action<DbContextOptionsBuilder>? optionsAction = null)
	{
		ArgumentNullException.ThrowIfNull(builder);
		ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

		_ = builder.Services.AddNpgsql<VrapDbContext>(connectionString, npgsqlOptionsAction, optionsAction);
	}
}
