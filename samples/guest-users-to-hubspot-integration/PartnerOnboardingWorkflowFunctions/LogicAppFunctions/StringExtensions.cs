using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LogicAppFunctions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string value)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(value.ToLower());
        }

        public static bool IsAllCaps(this string value)
        {
            return value.All(char.IsUpper);
        }
    }
}
