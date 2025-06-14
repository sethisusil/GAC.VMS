using Polly.Retry;
using Polly;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace GAC.WMS.Worker.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ResiliencePolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r =>
                    !r.IsSuccessStatusCode &&
                    (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.RequestTimeout
                )
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (outcome, timespan, attempt, context) =>
                    {
                        Console.WriteLine($"Retry {attempt} after {timespan.TotalSeconds}s for status {outcome.Result?.StatusCode}");
                    });
        }
    }
}
