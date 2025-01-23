using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Vrap.Database.LifeLog;
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

		ConfigureAbstractEntity<TableField, FieldType>(modelBuilder, nameof(TableField.FieldType));
		ConfigureAbstractEntity<FieldEntry, FieldType>(modelBuilder, nameof(FieldEntry.FieldType));

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(VrapDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

	private static void ConfigureAbstractEntity<TBase, TDiscriminator>(ModelBuilder modelBuilder, string discriminatorPropertyName)
		where TBase : class
	{
		var tbase = typeof(TBase);
		var tdiscriminator = typeof(TDiscriminator);

		HashSet<TDiscriminator> used = [];

		var discriminatorBuilder = modelBuilder.Entity<TBase>()
			.HasDiscriminator<TDiscriminator>(discriminatorPropertyName);

		foreach (var type in typeof(VrapDbContext).Assembly.GetTypes()
			.Where(type => type.BaseType == tbase))
		{
			if (type.GetInterface("IDiscriminatedChild`1") is not { } @interface
				|| @interface.GetGenericArguments().SingleOrDefault() != tdiscriminator)
			{
				throw new InvalidOperationException(
					$"Derived type {type} does not implemented interface {typeof(IDiscriminatedChild<TDiscriminator>)}");
			}

			var prop = @interface.GetProperty("Discriminator")
				?? throw new InvalidOperationException("Discriminator property not found");

			var value = prop.GetValue(null) ?? throw new InvalidOperationException("Could not get value");
			var value2 = (TDiscriminator)value;

			if (!used.Add(value2))
			{
				throw new InvalidOperationException($"Discriminator value {value2} is used more than once");
			}

			modelBuilder.Entity(type).HasBaseType(tbase);
			discriminatorBuilder.HasValue(type, value2);
		}
	}
}
