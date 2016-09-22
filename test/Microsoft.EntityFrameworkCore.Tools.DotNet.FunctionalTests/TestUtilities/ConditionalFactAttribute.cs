// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("Microsoft.EntityFrameworkCore.TestUtilities.ConditionalFactDiscoverer", "Tools.DotNet.FunctionalTests")]
    public class ConditionalFactAttribute : FactAttribute
    {
    }
}
