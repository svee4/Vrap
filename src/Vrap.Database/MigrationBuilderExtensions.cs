using Microsoft.EntityFrameworkCore.Migrations;

namespace Vrap.Database;

internal static class MigrationBuilderExtensions
{
	/// <summary>
	/// Creates a hypertable from an existing normal table.<br/>
	/// Note that this operation is not reversible
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="table"></param>
	/// <param name="column"></param>
	public static void CreateHyperTable(this MigrationBuilder builder, string table, string column) =>
		// yes you need to do the special quotes for the table name but not the column name dont ask me
		builder.Sql($"""SELECT create_hypertable('"{table}"', '{column}', if_not_exists => TRUE);""");
}
