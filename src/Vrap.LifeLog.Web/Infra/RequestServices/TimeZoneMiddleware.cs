namespace Vrap.LifeLog.Web.Infra.RequestServices;

public sealed partial class TimeZoneMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task InvokeAsync(HttpContext context, ILogger<TimeZoneMiddleware> logger)
	{
		InvokeInternal(context, logger);
		await _next(context);
	}

	private static void InvokeInternal(HttpContext context, ILogger<TimeZoneMiddleware> logger)
	{
		var reqTz = context.Request.Cookies["timezone"];

		if (string.IsNullOrEmpty(reqTz))
		{
			LogMissingCookie(logger);
			return;
		}

		if (!TimeZoneInfo.TryFindSystemTimeZoneById(reqTz, out var sysTz))
		{
			LogMissingSystemTimeZone(logger, reqTz);
			return;
		}

		context.Features.Set<ITimeZoneFeature>(new TimeZoneFeature(sysTz));
	}

	[LoggerMessage("No request timezone cookie provided", Level = LogLevel.Debug)]
	private static partial void LogMissingCookie(ILogger logger);

	[LoggerMessage("Could not find system timezone: {TimeZone}", Level = LogLevel.Debug)]
	private static partial void LogMissingSystemTimeZone(ILogger logger, string timeZone);

}
