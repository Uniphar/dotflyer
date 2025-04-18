﻿namespace DotFlyer.Api.Tests;

public static class Helpers
{
    public static async Task<TData> WaitSingleQueryResult<TData>(
        this ICslQueryProvider queryProvider,
        string query,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
        where TData : class
    {
        TData? result = null;

        await Policy.HandleResult<TData?>(r => r == null && !cancellationToken.IsCancellationRequested)
                    .WaitAndRetryAsync((int)timeout.TotalSeconds, _ => TimeSpan.FromSeconds(1))
                    .ExecuteAsync(async () =>
                    {
                        using var readerStream = await queryProvider!
                            .ExecuteQueryAsync(queryProvider.DefaultDatabaseName, query, default, cancellationToken);

                        var singleResult = readerStream.ToJObjects().SingleOrDefault();

                        if (singleResult != null)
                        {
                            result = singleResult.ToObject<TData>();
                        }

                        return result;
                    });


        if (result == null && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("No result");
        }

        return result!;
    }
}
