using System.Diagnostics.CodeAnalysis;

namespace Vrap.Database.LifeLog;

[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "This isnt VB")]
public enum FieldType
{
	DateTime = 1,
	Enum,
	Number,
	String
}
