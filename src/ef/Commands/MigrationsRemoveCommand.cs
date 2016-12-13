// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class MigrationsRemoveCommand : ContextCommandBase
    {
        protected override int Execute()
        {
            var deletedFiles = CreateExecutor().RemoveMigration(Context.Value(), _force.HasValue());
            if (_json.HasValue())
            {
                ReportJsonResults(deletedFiles);
            }

            return base.Execute();
        }

        private void ReportJsonResults(IEnumerable<string> files)
        {
            var output = new StringBuilder();
            output.AppendLine(Reporter.JsonPrefix);
            output.AppendLine("{");
            output.AppendLine("  \"files\": [");
            var first = true;
            foreach (var file in files)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output.AppendLine(",");
                }

                output.Append("    \"" + SerializePath(file) + "\"");
            }

            output.AppendLine();
            output.AppendLine("  ]");
            output.AppendLine("}");
            output.AppendLine(Reporter.JsonSuffix);
            Reporter.WriteInformation(output.ToString());
        }

        private static string SerializePath(string path)
            => path?.Replace("\\", "\\\\");
    }
}
