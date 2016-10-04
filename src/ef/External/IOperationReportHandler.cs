// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Design
{
    public interface IOperationReportHandler
    {
        int Version { get; }
        void OnWarning(string message);
        void OnInformation(string message);
        void OnVerbose(string message);
    }
}
