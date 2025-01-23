using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;
using Vrap.Database;
using Vrap.Database.LifeLog;
using Vrap.Database.LifeLog.Configuration;
using Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;
using Vrap.LifeLog.Web.Infra;
using Vrap.LifeLog.Web.Infra.Mvc;
using Vrap.Shared;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit;

[Route("DataTables/{id:int}/Edit")]
[MapView<EditViewModel>("./EditView")]
[MapView<NotFoundModel>("/Infra/Mvc/NotFound")]
[MapPartialView<AddFieldModalPartialModel>("./Partials/AddFieldModalPartial")]
[MapPartialView<FieldPartialModel>("./Partials/FieldPartial")]
[MapPartialView<FieldArgumentsPartialModel>("./Partials/FieldArgumentsPartial")]
public partial class EditController : MvcController
{
	[HttpGet("")]
	public IActionResult Get(int id, [FromServices] VrapDbContext dbContext)
	{
		var table = dbContext.DataTables
			.Include(table => table.Fields)
			.Select(table => new
			{
				Id = table.Id,
				Name = table.Name,
				Fields = table.Fields
					.OrderBy(field => field.Ordinal)
					.Select(field => new
					{
						Name = field.Name,
						Type = GetFieldType(field),
						Field = field
					})
			})
			.FirstOrDefault(table => table.Id == id);

		if (table is null)
		{
			return Result.NotFound($"/DataTables/{id}/Edit/", "/DataTables");
		}

		var model = new EditViewModel
		{
			Id = id,
			Table = new EditViewModel.DataTable
			{
				Name = table.Name,
				Fields = table.Fields.Select(field =>
					new FieldPartialModel(
						field.Name, field.Type, LifeLogHelpers.FieldArguments.FromField(field.Field), false))
				.ToList()
			}
		};

		return Views.EditView(model);
	}

	[HttpPost("AddField")]
	public async Task<IActionResult> AddFieldStub(int id, AddFieldModelBase model, [FromServices] VrapDbContext dbContext) =>
		model switch
		{
			AddDateTimeFieldModel dateTimeModel => await AddDateTimeField(id, dateTimeModel, dbContext),
			AddNumberFieldModel numberModel => await AddNumberField(id, numberModel, dbContext),
			AddStringFieldModel stringModel => await AddStringField(id, stringModel, dbContext),
			AddEnumFieldModel enumModel => await AddEnumField(id, enumModel, dbContext),
			_ => BadRequest("Invalid type")
		};

	private async Task<IActionResult> AddDateTimeField(int id, AddDateTimeFieldModel model, [FromServices] VrapDbContext dbContext)
	{
		var args = new DateTimeArguments(model.MinValue, model.MaxValue, model.Required, model.Ordinal);

		var err = await AddFieldHelper(id, model, dbContext, args,
			getField: () => DateTimeField.Create(model.Name, args.Required, args.Ordinal, args.MinValue, args.MaxValue));

		return err ?? Views.FieldPartial(new FieldPartialModel(model.Name, FieldType.DateTime, args, true));
	}

	private async Task<IActionResult> AddEnumField(int id, AddEnumFieldModel model, [FromServices] VrapDbContext dbContext)
	{
		var args = new EnumArguments(model.Options.Select(op => new EnumArguments.Option(-1, op)).ToList(), model.Required, model.Ordinal);

		var err = await AddFieldHelper(id, model, dbContext, args,
			getField: () => EnumField.Create(model.Name, args.Required, args.Ordinal,
				args.Options.Select(op => EnumOption.Create(op.Value, null))));

		return err ?? Views.FieldPartial(new FieldPartialModel(model.Name, FieldType.Enum, args, true));
	}

	private async Task<IActionResult> AddNumberField(int id, AddNumberFieldModel model, [FromServices] VrapDbContext dbContext)
	{
		var args = new NumberArguments(model.MinValue, model.MaxValue, model.Required, model.Ordinal);

		var err = await AddFieldHelper(id, model, dbContext, args,
			getField: () => NumberField.Create(model.Name, args.Required, args.Ordinal, args.MinValue, args.MaxValue));

		return err ?? Views.FieldPartial(new FieldPartialModel(model.Name, FieldType.Number, args, true));
	}

	private async Task<IActionResult> AddStringField(int id, AddStringFieldModel model, [FromServices] VrapDbContext dbContext)
	{
		var args = new StringArguments(model.MaxLength, model.Required, model.Ordinal);

		var err = await AddFieldHelper(id, model, dbContext, args,
			getField: () => StringField.Create(model.Name, args.Required, args.Ordinal, args.MaxLength));

		return err ?? Views.FieldPartial(new FieldPartialModel(model.Name, model.Type, args, true));
	}

	public async Task<IActionResult?> AddFieldHelper(
		int tableId,
		AddFieldModelBase model,
		VrapDbContext dbContext,
		FieldArguments args,
		Func<TableField> getField)
	{
		if (!ModelState.IsValid)
		{
			return Result
				.WithStatus(HttpStatusCode.BadRequest)
				.WithView(Views.AddFieldModalPartial(new AddFieldModalPartialModel
				{
					Name = model.Name,
					TableId = tableId,
					Type = model.Type,
					Required = model.Required,
					Ordinal = model.Ordinal,
					FieldTypes = FieldTypes,
					FieldArguments = args
				}))
				.ToActionResult();
		}

		var table = await dbContext.DataTables
			.Include(table => table.Fields)
			.FirstOrDefaultAsync(table => table.Id == tableId);

		if (table is null)
		{
			return BadRequest("Bad table id");
		}

		table.Fields.Add(getField());
		_ = await dbContext.SaveChangesAsync();
		return null;
	}


	[HttpGet("AddFieldModalPartial")]
	public IActionResult AddFieldPartial(int id) =>
		Views.AddFieldModalPartial(new AddFieldModalPartialModel
		{
			Name = "",
			TableId = id,
			Type = null,
			Required = false,
			Ordinal = 1,
			FieldTypes = FieldTypes,
			FieldArguments = null
		});

	[HttpGet("FieldArguments")]
	public IActionResult FieldArguments(FieldType type) =>
		Views.FieldArgumentsPartial(new FieldArgumentsPartialModel(type, null));

	public class AddFieldModelBinderProvider : IModelBinderProvider
	{
		private static IReadOnlyList<Type> AddFieldModelBaseDerivedTypes { get; } =
			typeof(EditController)
				.GetNestedTypes()
				.Where(type => type.IsSubclassOf(typeof(AddFieldModelBase)))
				.ToReadOnlyList();

		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (context.Metadata.ModelType != typeof(AddFieldModelBase))
			{
				return null!;
			}

			var subclasses = AddFieldModelBaseDerivedTypes;

			var binders = new Dictionary<Type, (ModelMetadata, IModelBinder)>();
			foreach (var type in subclasses)
			{
				var modelMetadata = context.MetadataProvider.GetMetadataForType(type);
				binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
			}

			return new AddFieldModelBinder(binders);
		}
	}

	public sealed class AddFieldModelBinder(Dictionary<Type, (ModelMetadata, IModelBinder)> binders) : IModelBinder
	{
		private readonly Dictionary<Type, (ModelMetadata, IModelBinder)> _binders = binders;

		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var modelTypeName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, "type");
			var modelTypeValue = bindingContext.ValueProvider.GetValue(modelTypeName).FirstValue;

			if (!Enum.TryParse<FieldType>(modelTypeValue, out var fieldType))
			{
				bindingContext.Result = ModelBindingResult.Failed();
				return;
			}

			var key = fieldType switch
			{
				FieldType.DateTime => typeof(AddDateTimeFieldModel),
				FieldType.Number => typeof(AddNumberFieldModel),
				FieldType.String => typeof(AddStringFieldModel),
				FieldType.Enum => typeof(AddEnumFieldModel),
				_ => throw new NotImplementedException(),
			};

			var (modelMetadata, modelBinder) = _binders[key];

			var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
				bindingContext.ActionContext,
				bindingContext.ValueProvider,
				modelMetadata,
				bindingInfo: null,
				bindingContext.ModelName);

			await modelBinder.BindModelAsync(newBindingContext);
			bindingContext.Result = newBindingContext.Result;

			if (newBindingContext.Result.IsModelSet && newBindingContext.Result.Model is { } model)
			{
				// Setting the ValidationState ensures properties on derived types are correctly 
				bindingContext.ValidationState[model] = new ValidationStateEntry
				{
					Metadata = modelMetadata,
				};
			}
		}
	}


	public abstract class AddFieldModelBase
	{
		public required string Name { get; init; }
		public required FieldType Type { get; init; }
		public bool Required { get; init; }
		public required int Ordinal { get; init; }
	}

	public sealed class AddDateTimeFieldModel : AddFieldModelBase
	{
		public DateTimeOffset? MinValue { get; init; }
		public DateTimeOffset? MaxValue { get; init; }
	}

	public sealed partial class AddEnumFieldModel : AddFieldModelBase, IValidatableObject
	{
		[MinLength(1)]
		[MaxLength(EnumField.OptionMaxLength)]
		public required IReadOnlyList<string> Options { get => _options; init => _options = value; }
		private IReadOnlyList<string> _options = null!;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var set = new HashSet<string>(Options.Count);

			foreach (var tempValue in Options)
			{
				if (string.IsNullOrWhiteSpace(tempValue))
				{
					yield return new ValidationResult("Option value cannot be empty or only whitespace");
					yield break;
				}

				var value = tempValue.Trim();

				bool success;

				try
				{
					value = ValidationHelpers.ValidateAndTrimAsPrintable(value, EnumOption.ValueMaxLength);
					success = true;
				}
				catch (RegexMatchTimeoutException)
				{
					success = false;
				}

				if (!success || value is null)
				{
					yield return new ValidationResult("Invalid option value");
					yield break;
				}

				if (!set.Add(value))
				{
					yield return new ValidationResult("Options cannot contain duplicate values");
					yield break;
				}
			}

			_options = [.. set];
		}
	}

	public sealed class AddNumberFieldModel : AddFieldModelBase
	{
		public decimal? MinValue { get; init; }
		public decimal? MaxValue { get; init; }
	}

	public sealed class AddStringFieldModel : AddFieldModelBase
	{
		[Range(1, StringField.AbsoluteMaxLength)]
		public int MaxLength { get; init; }
	}
}
