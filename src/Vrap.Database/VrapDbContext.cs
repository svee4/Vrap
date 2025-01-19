using Microsoft.EntityFrameworkCore;
using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;

namespace Vrap.Database;

public sealed partial class VrapDbContext(DbContextOptions options) : DbContext(options)
{
	public DbSet<DataTable> DataTables { get; private set; } = null!;
	public DbSet<TableField> TableFields { get; private set; } = null!;
	public DbSet<DataEntry> DataEntries { get; private set; } = null!;
	public DbSet<FieldEntry> FieldEntries { get; private set; } = null!;
	public DbSet<EnumOption> EnumOptions { get; private set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ArgumentNullException.ThrowIfNull(modelBuilder);

		MapDerivedTypes<TableField>(modelBuilder);
		MapDerivedTypes<FieldEntry>(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(VrapDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

	private static void MapDerivedTypes<TBase>(ModelBuilder modelBuilder)
	{
		var tbase = typeof(TBase);
		foreach (var type in typeof(VrapDbContext).Assembly.GetTypes()
			.Where(type => type.BaseType == tbase))
		{
			_ = modelBuilder.Entity(type).HasBaseType(tbase);
		}
	}
}
