using System.Text.RegularExpressions;

namespace Vrap.LifeLog.Web.Infra;

public static partial class ValidationHelpers
{

	/// <summary>
	/// Validates that string only has nice printable unicode word characters, then trims extra spaces.<br/>
	/// <paramref name="maxLength"/> check is done before trimming.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="maxLength"></param>
	/// <returns></returns>
	public static string? ValidateAndTrimAsPrintable(string input, int maxLength)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}

		// yes we do this check before trimming, dont care blehhh
		if (input.Length > maxLength)
		{
			return null;
		}

		if (!OnlyWordOrWhiteSpaceUnicodeRegex().IsMatch(input))
		{
			return null;
		}

		input = input.Trim();
		input = MoreThanOneWhitespaceBackToBackUnicodeRegex().Replace(input, " ");

		return input;
	}

	[GeneratedRegex(@"^[\w\s]+$", RegexOptions.None, matchTimeoutMilliseconds: 100)]
	public static partial Regex OnlyWordOrWhiteSpaceUnicodeRegex();

	[GeneratedRegex(@"\s{2,}", RegexOptions.None, matchTimeoutMilliseconds: 100)]
	public static partial Regex MoreThanOneWhitespaceBackToBackUnicodeRegex();
}
