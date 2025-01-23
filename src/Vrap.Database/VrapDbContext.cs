using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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

		ConfigureAbstractEntity<TableField, FieldType>(modelBuilder, m => m.FieldType, 0);
		ConfigureAbstractEntity<FieldEntry, FieldType>(modelBuilder, m => m.FieldType, 0);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(VrapDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

	private static void ConfigureAbstractEntity<TBase, TDiscriminator>(
			ModelBuilder modelBuilder,
			Expression<Func<TBase, TDiscriminator>> discriminatorPropertySelector,
			TDiscriminator baseTypeDiscriminator)
		where TBase : class
	{
		var tbase = typeof(TBase);
		var tdiscriminator = typeof(TDiscriminator);

		HashSet<TDiscriminator> used = [];

		var discriminatorBuilder = modelBuilder.Entity<TBase>()
			.HasDiscriminator<TDiscriminator>(discriminatorPropertySelector)
			.HasValue<TBase>(baseTypeDiscriminator);

		Console.WriteLine($"Configuring base {tbase} with discr {tdiscriminator}");

		foreach (var type in typeof(VrapDbContext).Assembly.GetTypes()
			.Where(type => type.BaseType == tbase))
		{

			var map = type.GetInterfaceMap(typeof(IDiscriminatedEntity<TDiscriminator>));
			var prop = map.TargetMethods.Single();

			var value = prop.Invoke(null, null) ?? throw new InvalidOperationException("Could not get value");
			var value2 = (TDiscriminator)value;

			if (!used.Add(value2))
			{
				throw new InvalidOperationException($"Discriminator value {value2} is used more than once");
			}

			Console.WriteLine($"Child {type} with discr {value2}");

			modelBuilder.Entity(type).HasBaseType(tbase);
			discriminatorBuilder.HasValue(type, value2);
		}
	}
}
