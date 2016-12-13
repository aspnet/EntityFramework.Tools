// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class DbContextScaffoldCommand : ProjectCommandBase
    {
        protected override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(_connection.Value))
            {
                throw new CommandException(string.Format(Resources.MissingArgument, _connection.Name));
            }
            if (string.IsNullOrEmpty(_provider.Value))
            {
                throw new CommandException(string.Format(Resources.MissingArgument, _provider.Name));
            }
        }

        protected override int Execute()
        {
            var filesCreated = CreateExecutor().ScaffoldContext(
                _provider.Value,
                _connection.Value,
                _outputDir.Value(),
                _context.Value(),
                _schemas.Values,
                _tables.Values,
                _dataAnnotations.HasValue(),
                _force.HasValue());
            if (_json.HasValue())
            {
                ReportJsonResults(filesCreated);
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
