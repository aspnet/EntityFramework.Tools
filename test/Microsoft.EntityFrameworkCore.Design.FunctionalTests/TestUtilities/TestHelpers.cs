// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.EntityFrameworkCore.Design.TestUtilities
{
    public class TestHelpers
    {
        private TestHelpers()
        {
        }

        public static TestHelpers Instance { get; } = new TestHelpers();

        public DbContextOptions CreateOptions(IServiceProvider serviceProvider = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder()
                .UseInternalServiceProvider(serviceProvider);

            UseProviderOptions(optionsBuilder);

            return optionsBuilder.Options;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection customServices = null)
            => CreateServiceProvider(customServices, AddProviderServices);

        private IServiceProvider CreateServiceProvider(
            IServiceCollection customServices,
            Func<IServiceCollection, IServiceCollection> addProviderServices)
        {
            var services = new ServiceCollection();
            addProviderServices(services);

            if (customServices != null)
            {
                foreach (var service in customServices)
                {
                    services.Add(service);
                }
            }

            return services.BuildServiceProvider();
        }

        public IServiceCollection AddProviderServices(IServiceCollection services) => services.AddEntityFrameworkInMemoryDatabase();

        private void UseProviderOptions(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase();

        public DbContext CreateContext()
            => new DbContext(CreateOptions(CreateServiceProvider()));

        public IServiceProvider CreateContextServices()
            => ((IInfrastructure<IServiceProvider>)CreateContext()).Instance;

        public ModelBuilder CreateConventionBuilder()
        {
            var contextServices = CreateContextServices();

            var conventionSetBuilder = contextServices.GetRequiredService<IDatabaseProviderServices>().ConventionSetBuilder;
            var conventionSet = contextServices.GetRequiredService<ICoreConventionSetBuilder>().CreateConventionSet();
            conventionSet = conventionSetBuilder == null
                ? conventionSet
                : conventionSetBuilder.AddConventions(conventionSet);
            return new ModelBuilder(conventionSet);
        }
    }
}
