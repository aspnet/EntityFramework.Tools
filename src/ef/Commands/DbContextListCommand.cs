// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class DbContextListCommand : ProjectCommandBase
    {
        protected override int Execute()
        {
            var types = CreateExecutor().GetContextTypes();

            if (_json.HasValue())
            {
                ReportJsonResults(types);
            }
            else
            {
                ReportResults(types);
            }

            return base.Execute();
        }

        private static void ReportJsonResults(IEnumerable<IDictionary> contextTypes)
        {
            var nameGroups = contextTypes.GroupBy(t => t["Name"]).ToList();
            var fullNameGroups = contextTypes.GroupBy(t => t["FullName"]).ToList();

            var output = new StringBuilder();

            output.AppendLine(Reporter.JsonPrefix);
            output.Append("[");

            var first = true;
            foreach (var contextType in contextTypes)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output.Append(",");
                }

                var safeName = nameGroups.Count(g => g.Key == contextType["Name"]) == 1
                    ? contextType["Name"]
                    : fullNameGroups.Count(g => g.Key == contextType["FullName"]) == 1
                        ? contextType["FullName"]
                        : contextType["AssemblyQualifiedName"];

                output.AppendLine();
                output.AppendLine("  {");
                output.AppendLine("     \"fullName\": \"" + contextType["FullName"] + "\",");
                output.AppendLine("     \"safeName\": \"" + safeName + "\",");
                output.AppendLine("     \"name\": \"" + contextType["Name"] + "\",");
                output.AppendLine("     \"assemblyQualifiedName\": \"" + contextType["AssemblyQualifiedName"] + "\"");
                output.Append("  }");
            }

            output.AppendLine();
            output.AppendLine("]");
            output.AppendLine(Reporter.JsonSuffix);

            Reporter.WriteInformation(output.ToString());
        }

        private static void ReportResults(IEnumerable<IDictionary> contextTypes)
        {
            var any = false;
            foreach (var contextType in contextTypes)
            {
                Reporter.WriteInformation(contextType["FullName"] as string);
                any = true;
            }

            if (!any)
            {
                Reporter.WriteInformation("No DbContext was found");
            }
        }
    }
}
