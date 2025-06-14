using GAC.WMS.Worker.Extensions;
using System.Text;
using System.Text.Json;

namespace GAC.WMS.Worker.HttpHelpers
{
    public abstract class BaseHttpClient(HttpClient _httpClient, ILogger<BaseHttpClient> _logger)
    {
        protected virtual async Task<T> PostAsync<T>(string url, object data, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(BaseHttpClient)}.{nameof(PostAsync)}: Initiating Http POST call with url:{url} and Request:{JsonSerializer.Serialize(data)}");
                var retryPolicy = ResiliencePolicies.GetRetryPolicy();
                var request= new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request, cancellationToken));
                var responseData= await response.Content.ReadAsStringAsync(cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<T>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                }
                else
                {
                    _logger.LogError($"{nameof(BaseHttpClient)}.{nameof(PostAsync)}:Error occurred while posting data to {url}. Status Code: {response.StatusCode}, Response: {responseData}");
                    return default(T)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(BaseHttpClient)}.{nameof(PostAsync)}:Error occurred while posting data to {url}. Error:{ex.Message}");
                return default(T)!;
            }
        }
    }
}
