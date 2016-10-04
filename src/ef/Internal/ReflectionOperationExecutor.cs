// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Tools.Internal
{
    public class ReflectionOperationExecutor : OperationExecutorBase
    {
        private readonly object _executor;
        private readonly Assembly _commandsAssembly;
        private const string ReportHandlerTypeName = "Microsoft.EntityFrameworkCore.Design.OperationReportHandler";
        private const string ResultHandlerTypeName = "Microsoft.EntityFrameworkCore.Design.OperationResultHandler";
        private readonly Type _resultHandlerType;

        public ReflectionOperationExecutor(
            string assembly,
            string startupAssembly,
            string projectDir,
            string contentRootPath,
            string dataDirectory,
            string rootNamespace,
            string environment)
            : base(assembly, startupAssembly, projectDir, contentRootPath, dataDirectory, rootNamespace, environment)
        {
            _commandsAssembly = Assembly.Load(new AssemblyName { Name = DesignAssemblyName });
            var reportHandlerType = _commandsAssembly.GetType(ReportHandlerTypeName, throwOnError: true, ignoreCase: false);

            var reportHandler = Activator.CreateInstance(
                reportHandlerType,
                (Action<string>)Reporter.Warning,
                (Action<string>)Reporter.Output,
                (Action<string>)Reporter.Verbose);

            _executor = Activator.CreateInstance(
                _commandsAssembly.GetType(ExecutorTypeName, throwOnError: true, ignoreCase: false),
                reportHandler,
                new Dictionary<string, string>
                {
                    { "targetName", AssemblyFileName },
                    { "startupTargetName", StartupAssemblyFileName },
                    { "projectDir", ProjectDirectory },
                    { "contentRootPath", ContentRootPath },
                    { "rootNamespace", RootNamespace },
                    { "environment", EnvironmentName }
                });

            _resultHandlerType = _commandsAssembly.GetType(ResultHandlerTypeName, throwOnError: true, ignoreCase: false);
        }

        protected override object CreateResultHandler()
            => Activator.CreateInstance(_resultHandlerType);

        protected override void Execute(string operationName, object resultHandler, IDictionary arguments)
            => Activator.CreateInstance(
                _commandsAssembly.GetType(ExecutorTypeName + "+" + operationName, throwOnError: true, ignoreCase: true),
                _executor,
                resultHandler,
                arguments);
    }
}
