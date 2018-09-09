// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.HealthChecks.Fakes;
using Xunit;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckServiceTest
    {
        readonly HealthCheckBuilder _builder = new HealthCheckBuilder();
        readonly FakeServiceProvider _serviceProvider = new FakeServiceProvider();

        public class CheckHealthAsync : HealthCheckServiceTest
        {
            [Fact]
            public async Task NoChecks_ReturnsUnknownStatus()
            {
                var service = new HealthCheckService(_builder, _serviceProvider, _serviceProvider.ScopeFactory);

                var result = await service.CheckHealthAsync();

                Assert.Equal(CheckStatus.Unknown, result.CheckStatus);
                Assert.Empty(result.Results);
            }

            [Fact]
            public async Task Checks_ReturnsCompositeStatus()
            {
                _builder.WithPartialSuccessStatus(CheckStatus.Warning)
                        .AddCheck("c1", () => HealthCheckResult.Healthy("Healthy check"))
                        .AddCheck("c2", () => HealthCheckResult.Unhealthy("Unhealthy check"));
                var service = new HealthCheckService(_builder, _serviceProvider, _serviceProvider.ScopeFactory);

                var result = await service.CheckHealthAsync();

                Assert.Equal(CheckStatus.Warning, result.CheckStatus);
                Assert.Collection(result.Results.OrderBy(kvp => kvp.Key).Select(ToDescriptiveString),
                    item => Assert.Equal("'c1' = 'Healthy (Healthy check)'", item),
                    item => Assert.Equal("'c2' = 'Unhealthy (Unhealthy check)'", item)
                );
            }

            [Fact]
            public async Task RunsScoped()
            {
                var service = new HealthCheckService(_builder, _serviceProvider, _serviceProvider.ScopeFactory);

                await service.CheckHealthAsync();

                Assert.Collection(_serviceProvider.Operations,
                    operation => Assert.Equal("Scope created", operation),
                    operation => Assert.Equal("Scope disposed", operation)
                );
            }

            [Fact]
            public async Task GroupsIntegrationTest()
            {
                _builder.WithPartialSuccessStatus(CheckStatus.Warning)
                        .AddCheck("c1", () => HealthCheckResult.Healthy("Healthy c1"))
                        .AddHealthCheckGroup("g1",
                                             group => group.AddCheck("gc1", () => HealthCheckResult.Unhealthy("Unhealthy gc1"))
                                                           .AddCheck("gc2", () => HealthCheckResult.Warning("Warning gc2")));
                var service = new HealthCheckService(_builder, _serviceProvider, _serviceProvider.ScopeFactory);

                var result = await service.CheckHealthAsync();

                Assert.Equal("Healthy (Healthy c1)", ToDescriptiveString(result.Results["c1"]));
                var groupResult = Assert.IsType<CompositeHealthCheckResult>(result.Results["Group(g1)"]);
                Assert.Equal(CheckStatus.Unhealthy, groupResult.CheckStatus);
                Assert.Collection(groupResult.Results.OrderBy(kvp => kvp.Key).Select(ToDescriptiveString),
                    item => Assert.Equal("'gc1' = 'Unhealthy (Unhealthy gc1)'", item),
                    item => Assert.Equal("'gc2' = 'Warning (Warning gc2)'", item)
                );
            }
        }

        static string ToDescriptiveString(IHealthCheckResult result)
            => $"{result.CheckStatus} ({result.Description})";

        static string ToDescriptiveString(KeyValuePair<string, IHealthCheckResult> kvp)
            => $"'{kvp.Key}' = '{ToDescriptiveString(kvp.Value)}'";
    }
}
