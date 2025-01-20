using System.ComponentModel.DataAnnotations;
using Vrap.Database.LifeLog.Configuration;

namespace Vrap.LifeLog.Web.Features.DataTables.Create;

public sealed class CreateModel
{
	[Required(AllowEmptyStrings = false)]
	[MinLength(1)]
	[MaxLength(DataTable.NameMaxLength)]
	public required string Name { get; init; }
}
