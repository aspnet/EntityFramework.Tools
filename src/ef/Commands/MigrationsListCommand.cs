// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class MigrationsListCommand : ContextCommandBase
    {
        protected override int Execute()
        {
            var migrations = CreateExecutor().GetMigrations(Context.Value());

            if (_json.HasValue())
            {
                ReportJsonResults(migrations);
            }
            else
            {
                ReportResults(migrations);
            }

            return base.Execute();
        }

        private static void ReportJsonResults(IEnumerable<IDictionary> migrations)
        {
            var nameGroups = migrations.GroupBy(m => m["Name"]).ToList();
            var output = new StringBuilder();
            output.AppendLine(Reporter.JsonPrefix);

            output.Append("[");

            var first = true;
            foreach (var m in migrations)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output.Append(",");
                }

                var safeName = nameGroups.Count(g => g.Key == m["Name"]) == 1
                    ? m["Name"]
                    : m["Id"];

                output.AppendLine();
                output.AppendLine("  {");
                output.AppendLine("    \"id\": \"" + m["Id"] + "\",");
                output.AppendLine("    \"name\": \"" + m["Name"] + "\",");
                output.AppendLine("    \"safeName\": \"" + safeName + "\"");
                output.Append("  }");
            }

            output.AppendLine();
            output.AppendLine("]");
            output.AppendLine(Reporter.JsonSuffix);

            Reporter.WriteInformation(output.ToString());
        }

        private static void ReportResults(IEnumerable<IDictionary> migrations)
        {
            var any = false;
            foreach (var migration in migrations)
            {
                Reporter.WriteInformation(migration["Id"] as string);
                any = true;
            }

            if (!any)
            {
                Reporter.WriteInformation("No migrations were found");
            }
        }
    }
}
