// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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

                if (result == null)
                {
                    return 0;
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
            if (result == null)
            {
                Reporter.WriteInformation("Could not find database to drop");
                return;
            }

            Reporter.WriteInformation("This command will permanently drop the database:");
            Reporter.WriteInformation(string.Empty);
            Reporter.WriteInformation($"    Database name : {result["DatabaseName"]}");
            Reporter.WriteInformation($"    Data source   : {result["DataSource"]}");
            Reporter.WriteInformation(string.Empty);
        }

        private static void ReportJsonDatabaseDiscovered(IDictionary result)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Reporter.JsonPrefix);

            if (result == null)
            {
                sb.AppendLine("{ }");
            }
            else
            {
                sb
                    .AppendLine("{")
                    .AppendLine("    \"databaseName\": " + SerializeString(result["DatabaseName"] as string) + ",")
                    .AppendLine("    \"dataSource\": " + SerializeString(result["DataSource"] as string))
                    .AppendLine("}");
            }

            sb.AppendLine(Reporter.JsonSuffix);

            Reporter.WriteInformation(sb.ToString());
        }

        private static readonly IReadOnlyDictionary<char, string> _replaces
            = new Dictionary<char, string>
            {
                { '\n', @"\n" },
                { '\t', @"\t" },
                { '\r', @"\r" },
                { '\f', @"\f" },
                { '\b', @"\b" },
                { '"', @"\""" },
                { '\\', @"\\" }
            };

        private static string SerializeString(string raw)
        {
            if (raw == null)
            {
                return "null";
            }

            var sb = new StringBuilder(raw.Length + 2);
            sb.Append('"');
            foreach (var c in raw)
            {
                if (_replaces.ContainsKey(c))
                {
                    sb.Append(_replaces[c]);
                }
                else
                {
                    sb.Append(c);
                }
            }

            sb.Append('"');

            return sb.ToString();
        }
    }
}
