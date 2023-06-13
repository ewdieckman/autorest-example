using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRestExample
{
    internal static class EnvironmentVariables
    {
        public static string? AuthorityHost => GetNonEmptyStringOrNull(Environment.GetEnvironmentVariable("EXAMPLE_AUTHORITY_HOST"));

        private static string? GetNonEmptyStringOrNull(string? str)
        {
            return !string.IsNullOrEmpty(str) ? str : null;
        }

        private static bool EnvironmentVariableToBool(string str)
        {
            return (string.Equals(bool.TrueString, str, StringComparison.OrdinalIgnoreCase) || string.Equals("1", str, StringComparison.OrdinalIgnoreCase));
        }
    }
}