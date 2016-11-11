// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tools.Console.Properties;

#if NET451
using System.Runtime.Remoting;
#endif

namespace Microsoft.EntityFrameworkCore.Tools.Internal
{
    public abstract class OperationExecutorBase : IOperationExecutor
    {
        private const string DataDirEnvName = "ADONET_DATA_DIR";
        public const string DesignAssemblyName = "Microsoft.EntityFrameworkCore.Design";
        protected const string ExecutorTypeName = "Microsoft.EntityFrameworkCore.Design.OperationExecutor";
        private const string OperationExceptionTypeName = "Microsoft.EntityFrameworkCore.Design.OperationException";

        private static readonly IDictionary EmptyArguments = new Dictionary<string, object>(0);
        public string AppBasePath { get; }

        protected string AssemblyFileName { get; set; }
        protected string StartupAssemblyFileName { get; set; }
        protected string ContentRootPath { get; }
        protected string ProjectDirectory { get; }
        protected string RootNamespace { get; }
        protected string EnvironmentName { get; }

        protected OperationExecutorBase(
            string assembly,
            string startupAssembly,
            string projectDir,
            string contentRootPath,
            string dataDirectory,
            string rootNamespace,
            string environment)
        {
            AssemblyFileName = Path.GetFileNameWithoutExtension(assembly);
            StartupAssemblyFileName = string.IsNullOrWhiteSpace(startupAssembly)
                ? AssemblyFileName
                : Path.GetFileNameWithoutExtension(startupAssembly);

            AppBasePath = Path.GetDirectoryName(assembly);
            if (!Path.IsPathRooted(AppBasePath))
            {
                AppBasePath = Path.Combine(Directory.GetCurrentDirectory(), AppBasePath);
            }

            Reporter.Verbose("Setting app base path " + AppBasePath);

            ContentRootPath = contentRootPath ?? AppBasePath;
            RootNamespace = rootNamespace ?? AssemblyFileName;
            ProjectDirectory = projectDir;
            EnvironmentName = environment;

            if (!string.IsNullOrEmpty(dataDirectory))
            {
                Environment.SetEnvironmentVariable(DataDirEnvName, dataDirectory);
            }
        }

        public virtual void Dispose()
        {
        }

        protected abstract object CreateResultHandler();
        protected abstract void Execute(string operationName, object resultHandler, IDictionary arguments);

        private TResult InvokeOperation<TResult>(string operation)
            => InvokeOperation<TResult>(operation, EmptyArguments);

        private TResult InvokeOperation<TResult>(string operation, IDictionary arguments)
            => (TResult)InvokeOperationImpl(operation, arguments);

        private void InvokeOperation(string operation, IDictionary arguments)
            => InvokeOperationImpl(operation, arguments, true);

        private object InvokeOperationImpl(string operationName, IDictionary arguments, bool isVoid = false)
        {
            var resultHandler = (dynamic)CreateResultHandler();

            Execute(operationName, resultHandler, arguments);

            if (resultHandler.ErrorType != null)
            {
                if (resultHandler.ErrorType == OperationExceptionTypeName)
                {
                    Reporter.Verbose(resultHandler.ErrorStackTrace);
                }
#if NET451
                else if (resultHandler.ErrorType == typeof(RemotingException).FullName)
                {
                    Reporter.Verbose(resultHandler.ErrorStackTrace);
                    throw new OperationErrorException(ToolsStrings.DesignDependencyIncompatible);
                }
#endif
                else
                {
                    Reporter.Verbose(resultHandler.ErrorStackTrace);
                }

                throw new OperationErrorException(resultHandler.ErrorMessage);
            }

            if (!isVoid
                && !resultHandler.HasResult)
            {
                throw new InvalidOperationException(
                    $"A value was not returned for operation '{operationName}'.");
            }

            return resultHandler.Result;
        }

        public IDictionary AddMigration(string name, string outputDir, string contextType)
            => InvokeOperation<IDictionary>("AddMigration",
                new Dictionary<string, string>
                {
                    ["name"] = name,
                    ["outputDir"] = outputDir,
                    ["contextType"] = contextType
                });

        public IEnumerable<string> RemoveMigration(string contextType, bool force)
            => InvokeOperation<IEnumerable<string>>("RemoveMigration",
                new Dictionary<string, object>
                {
                    ["contextType"] = contextType,
                    ["force"] = force
                });

        public IEnumerable<IDictionary> GetMigrations(string contextType)
            => InvokeOperation<IEnumerable<IDictionary>>("GetMigrations",
                new Dictionary<string, object>
                {
                    ["contextType"] = contextType
                });

        public void DropDatabase(string contextType)
            => InvokeOperation("DropDatabase",
                new Dictionary<string, object>
                {
                    ["contextType"] = contextType
                });

        public IDictionary GetContextInfo(string name)
            => InvokeOperation<IDictionary>("GetContextInfo",
                new Dictionary<string, object>
                {
                    ["contextType"] = name
                });

        public void UpdateDatabase(string migration, string contextType)
            => InvokeOperation("UpdateDatabase",
                new Dictionary<string, string>
                {
                    ["targetMigration"] = migration,
                    ["contextType"] = contextType
                });

        public IEnumerable<IDictionary> GetContextTypes()
            => InvokeOperation<IEnumerable<IDictionary>>("GetContextTypes");

        public IEnumerable<string> ScaffoldContext(string provider,
            string connectionString,
            string outputDir,
            string dbContextClassName,
            IEnumerable<string> schemaFilters,
            IEnumerable<string> tableFilters,
            bool useDataAnnotations,
            bool overwriteFiles)
            => InvokeOperation<IEnumerable<string>>("ScaffoldContext",
                new Dictionary<string, object>
                {
                    ["provider"] = provider,
                    ["connectionString"] = connectionString,
                    ["outputDir"] = outputDir,
                    ["dbContextClassName"] = dbContextClassName,
                    ["schemaFilters"] = schemaFilters,
                    ["tableFilters"] = tableFilters,
                    ["useDataAnnotations"] = useDataAnnotations,
                    ["overwriteFiles"] = overwriteFiles
                });

        public string ScriptMigration(
            string fromMigration,
            string toMigration,
            bool idempotent,
            string contextType)
            => InvokeOperation<string>("ScriptMigration",
                new Dictionary<string, object>
                {
                    ["fromMigration"] = fromMigration,
                    ["toMigration"] = toMigration,
                    ["idempotent"] = idempotent,
                    ["contextType"] = contextType
                });

        public string GetContextType(string name)
            => InvokeOperation<string>("GetContextType",
                new Dictionary<string, string>
                {
                    ["name"] = name
                });
    }
}
