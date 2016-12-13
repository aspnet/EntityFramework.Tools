// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET451

using System;
using Microsoft.EntityFrameworkCore.Tools;

namespace Microsoft.EntityFrameworkCore.Design
{
    public class OperationReportHandler : MarshalByRefObject, IOperationReportHandler
    {
        public int Version
            => 0;

        public void OnError(string message)
            => Reporter.WriteError(message);

        public void OnWarning(string message)
            => Reporter.WriteWarning(message);

        public void OnInformation(string message)
            => Reporter.WriteInformation(message);

        public void OnVerbose(string message)
            => Reporter.WriteVerbose(message);
    }
}

#endif
