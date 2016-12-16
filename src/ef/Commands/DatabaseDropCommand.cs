// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class DatabaseDropCommand : ContextCommandBase
    {
        protected override int Execute()
        {
            var executor = CreateExecutor();

            if (!_force.HasValue())
            {
                var result = executor.GetContextInfo(Context.Value());

                if (_json.HasValue())
                {
                    ReportJsonDatabaseDiscovered(result);
                }
                else
                {
                    ReportDatabaseDiscovered(result);
                }
            }

            if (_dryRun.HasValue())
            {
                return 0;
            }

            if (!_force.HasValue())
            {
                Reporter.WriteInformation("Are you sure you want to proceed? (y/N)");
                var readedKey = Console.ReadLine().Trim();
                var confirmed = (readedKey == "y") || (readedKey == "Y");
                if (!confirmed)
                {
                    Reporter.WriteInformation("Cancelled");
                    return 0;
                }
            }

            executor.DropDatabase(Context.Value());

            return base.Execute();
        }

        private static void ReportDatabaseDiscovered(IDictionary result)
        {
            Reporter.WriteInformation("This command will permanently drop the database:");
            Reporter.WriteInformation(string.Empty);
            Reporter.WriteInformation("    Database name : " + result["DatabaseName"]);
            Reporter.WriteInformation("    Data source   : " + result["DataSource"]);
            Reporter.WriteInformation(string.Empty);
        }

        private static void ReportJsonDatabaseDiscovered(IDictionary result)
        {
            Reporter.WriteData("{");
            Reporter.WriteData("  \"databaseName\": \"" + Json.Escape(result["DatabaseName"] as string) + "\",");
            Reporter.WriteData("  \"dataSource\": \"" + Json.Escape(result["DataSource"] as string) + "\"");
            Reporter.WriteData("}");
        }
    }
}
