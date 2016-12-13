// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Text;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class MigrationsAddCommand : ContextCommandBase
    {
        protected override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(_name.Value))
            {
                throw new CommandException(string.Format(Resources.MissingArgument, _name.Name));
            }
        }

        protected override int Execute()
        {
            var files = CreateExecutor().AddMigration(_name.Value, _outputDir.Value(), Context.Value());

            if (_json.HasValue())
            {
                ReportJson(files);
            }
            else
            {
                Reporter.WriteInformation("Done. To undo this action, use 'dotnet ef migrations remove'");
            }

            return base.Execute();
        }

        private static void ReportJson(IDictionary files)
        {
            var output = new StringBuilder();
            output.AppendLine(Reporter.JsonPrefix);
            output.AppendLine("{");
            output.AppendLine("  \"MigrationFile\": \"" + SerializePath(files["MigrationFile"] as string) + "\",");
            output.AppendLine("  \"MetadataFile\": \"" + SerializePath(files["MetadataFile"] as string) + "\",");
            output.AppendLine("  \"SnapshotFile\": \"" + SerializePath(files["SnapshotFile"] as string) + "\"");
            output.AppendLine("}");
            output.AppendLine(Reporter.JsonSuffix);
            Reporter.WriteInformation(output.ToString());
        }

        private static string SerializePath(string path)
            => path?.Replace("\\", "\\\\");
    }
}
