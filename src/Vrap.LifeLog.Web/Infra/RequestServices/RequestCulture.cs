using System.Globalization;

namespace Vrap.LifeLog.Web.Infra.RequestServices;

// this is scoped and kind of fake because its not actually from the request
// but its the best i care to do for now
public sealed class RequestCulture
{
	public CultureInfo? Culture { get; }

	public RequestCulture(IConfiguration config, ILogger<RequestCulture> logger)
	{
		if (config["Vrap:LifeLog:Culture"] is { } culture)
		{
			logger.LogDebug("Using culture {Culture}", culture);
			Culture = new CultureInfo(culture);
		}
		else
		{
			logger.LogDebug("No culture configured");
		}
	}
}
