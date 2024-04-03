namespace DotFlyer.Service.Tests;

public static class Helpers
{
    public static async Task<TData> WaitSingleQueryResult<TData>(this ICslQueryProvider queryProvider, string query, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        TData? result = default;

        Task queryResultCheck = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var readerStream = await queryProvider!
                    .ExecuteQueryAsync(queryProvider.DefaultDatabaseName, query, default);

                var singleResult = readerStream.ToJObjects().SingleOrDefault();

                if (singleResult != null)
                {
                    result = singleResult.ToObject<TData>();

                    break;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }, cancellationToken);

        var completedTask = await Task.WhenAny(queryResultCheck, Task.Delay(timeout, cancellationToken));

        if (completedTask != queryResultCheck && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("No result");
        }

        return result!;
    }
}
