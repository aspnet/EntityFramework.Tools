// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Tools.Utilities;

using static Microsoft.EntityFrameworkCore.Tools.Utilities.AnsiConstants;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Reporter
    {
        public static bool IsVerbose { get; set; }

        public static void WriteError(string message)
            => WriteToError(Bold + Red + message + Reset);

        public static void WriteWarning(string message)
            => WriteToOut(Bold + Yellow + message + Reset);

        public static void WriteInformation(string message)
            => WriteToOut(message);

        public static void WriteVerbose(string message)
        {
            if (IsVerbose)
            {
                WriteToOut(Bold + Black + message + Reset);
            }
        }

        private static void WriteToOut(string value)
            => AnsiConsole.WriteLine(value);

        private static void WriteToError(string value)
            => AnsiConsole.Error.WriteLine(value);
    }
}
