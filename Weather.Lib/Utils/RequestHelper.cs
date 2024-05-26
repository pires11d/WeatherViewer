using RestSharp;
using System.Text.Json;

namespace Weather.Lib.Utils
{
    public static class RequestHelper
    {
        private static RestClient? _client { get; set; } = null;

        public static RestClient ConfigureClient(string baseUrl)
        {
            return _client ?? (_client = new RestClient(baseUrl));
        }

        public static async Task<R?> SendAsync<T, R>(List<QueryParameter> queryParams, Method method, T command)
        {
            if (_client is null)
                throw new Exception("O Client não foi encontrado! Revise as configurações da API.");

            var request = CreateRequest<T>(queryParams, method, command);

            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessStatusCode)
            {
                R? result;
                try
                {
                    result = !string.IsNullOrEmpty(response.Content) ? JsonSerializer.Deserialize<R>(response.Content) : default;
                }
                catch (Exception ex)
                {
                    result = default;
                }
                return result;
            }

            throw new HttpRequestException(response.ErrorMessage);
        }

        private static RestRequest CreateRequest<T>(List<QueryParameter> queryParams, Method method, T command)
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = method;

            foreach (var queryParam in queryParams)
            {
                if (queryParam.Name != null && queryParam.Value != null)
                    request.AddParameter(queryParam);
            }

            if (command != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddBody(JsonSerializer.Serialize(command));
            }

            return request;
        }

        public static bool IsAlive(string publicUrl)
        {
            try
            {
                var request = new RestRequest(publicUrl);
                request.Method = Method.Get;
                var response = _client?.Execute<object>(request);
                return response?.IsSuccessStatusCode ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}
