// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckResult : IHealthCheckResult
    {
        static readonly IReadOnlyDictionary<string, object> _emptyData = new Dictionary<string, object>();

        public CheckStatus CheckStatus { get; }
        public long? Duration { get; }
        public IReadOnlyDictionary<string, object> Data { get; }
        public string Description { get; }

        HealthCheckResult(CheckStatus checkStatus, string description, IReadOnlyDictionary<string, object> data, long? duration)
        {
            CheckStatus = checkStatus;
            Description = description;
            Data = data ?? _emptyData;
            Duration = duration;
        }

        public static HealthCheckResult Unhealthy(string description)
            => new HealthCheckResult(CheckStatus.Unhealthy, description, null, null);

        public static HealthCheckResult Unhealthy(string description, IReadOnlyDictionary<string, object> data)
            => new HealthCheckResult(CheckStatus.Unhealthy, description, data, null);

        public static HealthCheckResult Unhealthy(string description, IReadOnlyDictionary<string, object> data, long? duration)
            => new HealthCheckResult(CheckStatus.Unhealthy, description, data, duration);


        public static HealthCheckResult Healthy(string description)
            => new HealthCheckResult(CheckStatus.Healthy, description, null, null);

        public static HealthCheckResult Healthy(string description, IReadOnlyDictionary<string, object> data)
            => new HealthCheckResult(CheckStatus.Healthy, description, data, null);

        public static HealthCheckResult Healthy(string description, IReadOnlyDictionary<string, object> data, long? duration)
            => new HealthCheckResult(CheckStatus.Healthy, description, data, duration);


        public static HealthCheckResult Warning(string description)
            => new HealthCheckResult(CheckStatus.Warning, description, null, null);

        public static HealthCheckResult Warning(string description, IReadOnlyDictionary<string, object> data)
            => new HealthCheckResult(CheckStatus.Warning, description, data, null);

        public static HealthCheckResult Warning(string description, IReadOnlyDictionary<string, object> data, long? duration)
            => new HealthCheckResult(CheckStatus.Warning, description, data, duration);


        public static HealthCheckResult Unknown(string description)
            => new HealthCheckResult(CheckStatus.Unknown, description, null, null);

        public static HealthCheckResult Unknown(string description, IReadOnlyDictionary<string, object> data)
            => new HealthCheckResult(CheckStatus.Unknown, description, data, null);

        public static HealthCheckResult Unknown(string description, IReadOnlyDictionary<string, object> data, long? duration)
            => new HealthCheckResult(CheckStatus.Unknown, description, data, duration);


        public static HealthCheckResult FromStatus(CheckStatus status, string description)
            => new HealthCheckResult(status, description, null, null);

        public static HealthCheckResult FromStatus(CheckStatus status, string description, IReadOnlyDictionary<string, object> data)
            => new HealthCheckResult(status, description, data, null);

        public static HealthCheckResult FromStatus(CheckStatus status, string description, IReadOnlyDictionary<string, object> data, long? duration)
            => new HealthCheckResult(status, description, data, duration);
    }
}
