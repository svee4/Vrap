using Microsoft.Extensions.Configuration;

namespace Vrap.Shared;

public static class ConfigurationExtensions
{
	public static string GetRequiredConfiguration(this IConfiguration configuration, string key)
	{
		ArgumentNullException.ThrowIfNull(configuration);
		ArgumentException.ThrowIfNullOrEmpty(key);

		var value = configuration[key];
		if (string.IsNullOrEmpty(value))
		{
			ConfigurationException.ThrowMissingKey(key);
		}

		return value;
	}

	public static T GetRequiredParsedConfiguration<T>(
		this IConfiguration configuration,
		string key,
		IFormatProvider? formatProvider = null) where T : IParsable<T>
	{
		ArgumentNullException.ThrowIfNull(configuration);
		ArgumentException.ThrowIfNullOrWhiteSpace(key);

		var stringValue = configuration.GetRequiredConfiguration(key);

		if (!T.TryParse(stringValue, formatProvider, out var parsedValue))
		{
			ConfigurationException.ThrowUnparsable<T>(key);
		}

		return parsedValue;
	}
}
