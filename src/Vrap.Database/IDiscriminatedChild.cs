namespace Vrap.Database;

internal interface IDiscriminatedChild<TDiscriminator>
{
	static abstract TDiscriminator Discriminator { get; }
}
