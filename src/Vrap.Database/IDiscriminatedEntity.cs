namespace Vrap.Database;

internal interface IDiscriminatedEntity<TDiscriminator>
{
	static abstract TDiscriminator Discriminator { get; }
}
