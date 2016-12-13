// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class MigrationsScriptCommand : ContextCommandBase
    {
        protected override int Execute()
        {
            var sql = CreateExecutor().ScriptMigration(
                _from.Value,
                _to.Value,
                _idempotent.HasValue(),
                Context.Value());

            if (!_output.HasValue())
            {
                Reporter.WriteInformation(sql);
            }
            else
            {
                var directory = Path.GetDirectoryName(_output.Value());
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                Reporter.WriteVerbose("Writing SQL script to '" + _output.Value() + "'");
                File.WriteAllText(_output.Value(), sql, Encoding.UTF8);

                Reporter.WriteInformation("Done");
            }

            return base.Execute();
        }
    }
}
