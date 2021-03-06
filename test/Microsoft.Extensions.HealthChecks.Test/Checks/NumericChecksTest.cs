﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.HealthChecks.Fakes;
using Xunit;

namespace Microsoft.Extensions.HealthChecks.Checks
{
    public class NumericChecksTest
    {
        readonly HealthCheckBuilder _builder = new HealthCheckBuilder();
        readonly IServiceProvider _serviceProvider = new FakeServiceProvider();

        public class AddMinValueCheck : NumericChecksTest
        {
            [Fact]
            public void GuardClauses()
            {
                Assert.Throws<ArgumentNullException>("builder", () => HealthCheckBuilderExtensions.AddMinValueCheck(null, "name", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("name", () => HealthCheckBuilderExtensions.AddMinValueCheck(_builder, null, 42, () => 2112));
                Assert.Throws<ArgumentException>("name", () => HealthCheckBuilderExtensions.AddMinValueCheck(_builder, "", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("currentValueFunc", () => HealthCheckBuilderExtensions.AddMinValueCheck(_builder, "name", 42, null));
            }

            [Theory]
            [InlineData(-1, CheckStatus.Unhealthy)]
            [InlineData(1, CheckStatus.Healthy)]
            public async Task RegistersCheck(int monitoredValue, CheckStatus expectedStatus)
            {
                _builder.AddMinValueCheck("CheckName", 0, () => monitoredValue);

                var check = _builder.ChecksByName["CheckName"];

                var result = await check.RunAsync(_serviceProvider);
                Assert.Equal(expectedStatus, result.CheckStatus);
                Assert.Equal($"min=0, current={monitoredValue}", result.Description);
                Assert.Collection(result.Data.OrderBy(kvp => kvp.Key),
                    kvp =>
                    {
                        Assert.Equal("current", kvp.Key);
                        Assert.Equal(monitoredValue, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal("min", kvp.Key);
                        Assert.Equal(0, kvp.Value);
                    }
                );
            }
        }

        public class AddMaxValueCheck : NumericChecksTest
        {
            [Fact]
            public void GuardClauses()
            {
                Assert.Throws<ArgumentNullException>("builder", () => HealthCheckBuilderExtensions.AddMaxValueCheck(null, "name", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("name", () => HealthCheckBuilderExtensions.AddMaxValueCheck(_builder, null, 42, () => 2112));
                Assert.Throws<ArgumentException>("name", () => HealthCheckBuilderExtensions.AddMaxValueCheck(_builder, "", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("currentValueFunc", () => HealthCheckBuilderExtensions.AddMaxValueCheck(_builder, "name", 42, null));
            }

            [Theory]
            [InlineData(1, CheckStatus.Unhealthy)]
            [InlineData(-1, CheckStatus.Healthy)]
            public async Task RegistersCheck(int monitoredValue, CheckStatus expectedStatus)
            {
                _builder.AddMaxValueCheck("CheckName", 0, () => monitoredValue);

                var check = _builder.ChecksByName["CheckName"];

                var result = await check.RunAsync(_serviceProvider);
                Assert.Equal(expectedStatus, result.CheckStatus);
                Assert.Equal($"max=0, current={monitoredValue}", result.Description);
                Assert.Collection(result.Data.OrderBy(kvp => kvp.Key),
                    kvp =>
                    {
                        Assert.Equal("current", kvp.Key);
                        Assert.Equal(monitoredValue, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal("max", kvp.Key);
                        Assert.Equal(0, kvp.Value);
                    }
                );
            }
        }
    }
}
