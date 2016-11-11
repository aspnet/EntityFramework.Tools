// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.EntityFrameworkCore.Tools.Utilities
{
    internal class AnsiConsole
    {
        public static AnsiTextWriter Error { get; } = new AnsiTextWriter(Console.Error);
        public static AnsiTextWriter Out { get; } = new AnsiTextWriter(Console.Out);

        public static void WriteLine(string format, params object[] args)
            => Out.WriteLine(format, args);
    }
}
