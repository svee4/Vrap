namespace Vrap.LifeLog.Web.Infra.Mvc;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", 
	"CA1032:Implement standard exception constructors", Justification = "Dont want them pal")]
public class ServerException(string message) : Exception(message);
