using System.Globalization;

namespace TheCodeKitchen.Application.Business.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string input)
        => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.ToLower());
}