// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using static Microsoft.EntityFrameworkCore.Tools.AnsiConstants;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Reporter
    {
        public const string JsonPrefix = "//BEGIN";
        public const string JsonSuffix = "//END";

        public static bool IsVerbose { get; set; }
        public static bool NoColor { get; set; }
        public static bool PrefixOutput { get; set; }

        public static string Colorize(string value, Func<string, string> colorizeFunc)
            => NoColor ? value : colorizeFunc(value);

        public static void WriteError(string message)
            => WriteToError(Prefix("ERROR   : ", Colorize(message, x => Bold + Red + x + Reset)));

        public static void WriteWarning(string message)
            => WriteToOut(Prefix("WARNING : ", Colorize(message, x => Bold + Yellow + x + Reset)));

        public static void WriteInformation(string message)
            => WriteToOut(Prefix("OUTPUT  : ", message));

        public static void WriteVerbose(string message)
        {
            if (IsVerbose)
            {
                WriteToOut(Prefix("VERBOSE : ", Colorize(message, x => Bold + Black + x + Reset)));
            }
        }

        private static string Prefix(string prefix, string value)
            => PrefixOutput
                ? string.Join(
                    Environment.NewLine,
                    value.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(l => prefix + l))
                : value;

        private static void WriteToOut(string value)
        {
            if (NoColor)
            {
                Console.WriteLine(value);
            }
            else
            {
                AnsiConsole.WriteLine(value);
            }
        }

        private static void WriteToError(string value)
        {
            if (NoColor)
            {
                Console.Error.WriteLine(value);
            }
            else
            {
                AnsiConsole.Error.WriteLine(value);
            }
        }
    }
}
