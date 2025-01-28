using Vrap.LifeLog.Web.Infra.RequestServices;

namespace Vrap.LifeLog.Web.Infra;

public static class StartupExtensions
{
	public static void UseTimeZoneMiddleware(this WebApplication app) =>
		app.UseMiddleware<TimeZoneMiddleware>();
}
