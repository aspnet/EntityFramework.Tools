// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET451

using System;
using Microsoft.EntityFrameworkCore.Design;

namespace Microsoft.EntityFrameworkCore.Tools.Internal
{
    public class OperationReportHandler : MarshalByRefObject, IOperationReportHandler
    {
        public int Version
            => 0;

        public void OnError(string message)
            => Reporter.Error(message);

        public void OnWarning(string message)
            => Reporter.Warning(message);

        public void OnInformation(string message)
            => Reporter.Output(message);

        public void OnVerbose(string message)
            => Reporter.Verbose(message);
    }
}

#endif
