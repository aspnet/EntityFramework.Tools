// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet
{
    /// <summary>
    ///     Represents an exception whose stack trace should, by default, not be reported by the commands.
    /// </summary>
    public class OperationErrorException : Exception
    {
        public OperationErrorException(string message)
            : base(message)
        {
        }

        public OperationErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
