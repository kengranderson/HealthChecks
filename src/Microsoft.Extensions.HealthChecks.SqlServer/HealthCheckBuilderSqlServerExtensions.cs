// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Extensions.HealthChecks
{
    // REVIEW: What are the appropriate guards for these functions?

    public static class HealthCheckBuilderSqlServerExtensions
    {
        public static HealthCheckBuilder AddSqlCheck(this HealthCheckBuilder builder, string name, string connectionString,
            string procedureName, object parameter)
        {
            return AddSqlCheck(builder, name, connectionString, new string[] { procedureName}, new object[] { parameter}, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddSqlCheck(this HealthCheckBuilder builder, string name, string connectionString,
            string[] procedureNames, object[] parameters)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return AddSqlCheck(builder, name, connectionString, procedureNames, parameters, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddSqlCheck(this HealthCheckBuilder builder, string name, 
            string connectionString, string[] procedureNames, object[] parameters, TimeSpan cacheDuration)
        {
            builder.AddCheck($"SqlCheck({name})", async () =>
            {
                var timer = new Stopwatch();
                timer.Start();

                try
                {
                    //TODO: There is probably a much better way to do this.
                    using (var db = new SqlConnection(connectionString))
                    {
                        bool success = true;

                        for (int index = 0; index < procedureNames.Length && success; index++) {
                            string procedureName = procedureNames[index];
                            object parms = parameters[index];

                            var queryResult = await db.QueryAsync(procedureName, parms, commandType: CommandType.StoredProcedure);
                            success &= queryResult.Count() > 0;
                        }

                        timer.Stop();

                        if (success) {
                            return HealthCheckResult.Healthy($"SqlCheck({name}): Healthy", null, timer.ElapsedMilliseconds);
                        }
                        return HealthCheckResult.Unhealthy($"SqlCheck({name}): Unhealthy", null, timer.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"SqlCheck({name}): Exception during check: {ex.GetType().FullName}", null, timer.ElapsedMilliseconds);
                }
            }, cacheDuration);

            return builder;
        }
    }
}
