using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HotelCatalogService.Domain.Common.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            string str = phrase.ToLowerInvariant();

            str = str.Replace("đ", "d");

            str = str.Normalize(NormalizationForm.FormD);

            var chars = str
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            str = new string(chars).Normalize(NormalizationForm.FormC);

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", "-");

            str = str.Trim('-');

            return str;
        }
    }
}
