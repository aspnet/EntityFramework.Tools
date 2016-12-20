// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    internal abstract class ProjectCommandBase : EnvironmentCommandBase
    {
        private CommandOption _assembly;
        private CommandOption _startupAssembly;
        private CommandOption _dataDir;
        private CommandOption _projectDir;
        private CommandOption _contentRoot;
        private CommandOption _rootNamespace;
        private CommandOption _noAppDomain;

        public override void Configure(CommandLineApplication command)
        {
            _assembly = command.Option(
                "-a|--assembly <PATH>",
                "The assembly to use.");
            _noAppDomain = command.Option(
                "--no-appdomain",
                "Don't use app domains. Always implied on .NET Core.");
            _startupAssembly = command.Option(
                "-s|--startup-assembly <PATH>",
                "The startup assembly to use. Defaults to the target assembly.");
            _dataDir = command.Option(
                "--data-dir <PATH>",
                "The data directory. Defaults to the startup assembly directory.");
            _projectDir = command.Option(
                "--project-dir <PATH>",
                "The project directory. Defaults to the current directory.");
            _contentRoot = command.Option(
                "--content-root <PATH>",
                "The content root path. Defaults to the startup assembly directory.");
            _rootNamespace = command.Option(
                "--root-namespace <NAMESPACE>",
                "The root namespace. Defaults to the target assembly name.");

            base.Configure(command);
        }

        protected override void Validate()
        {
            base.Validate();

            if (!_assembly.HasValue())
            {
                throw new CommandException(string.Format(Resources.MissingOption, _assembly.LongName));
            }
        }

        protected IOperationExecutor CreateExecutor()
        {
            // TODO: Re-throw TypeLoadException and FileNotFoundException?
#if NET451
            if (!_noAppDomain.HasValue())
            {
                return new AppDomainOperationExecutor(
                    _assembly.Value(),
                    _startupAssembly.Value(),
                    _projectDir.Value(),
                    _contentRoot.Value(),
                    _dataDir.Value(),
                    _rootNamespace.Value(),
                    Environment.Value());
            }
#endif
            return new ReflectionOperationExecutor(
                _assembly.Value(),
                _startupAssembly.Value(),
                _projectDir.Value(),
                _contentRoot.Value(),
                _dataDir.Value(),
                _rootNamespace.Value(),
                Environment.Value());
        }
    }
}
