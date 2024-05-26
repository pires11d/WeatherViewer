using RestSharp;
using System.Runtime.Serialization;
using System.Text.Json;
using Weather.Lib.Utils.Interfaces;

namespace Weather.Lib.Utils
{
    public class RequestHelper : IRequestHelper
    {
        private static RestClient? _client { get; set; } = null;

        public static RestClient ConfigureClient(string baseUrl)
        {
            return _client ?? (_client = new RestClient(baseUrl));
        }

        public async Task<R?> SendAsync<T, R>(List<QueryParameter> queryParams, Method method, T command)
        {
            if (_client is null)
                throw new ApplicationException("O Client não foi encontrado! Revise as configurações da API.");

            var request = CreateRequest<T>(queryParams, method, command);

            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return !string.IsNullOrEmpty(response.Content) ? JsonSerializer.Deserialize<R>(response.Content) : default;
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Ocorreu um erro na desserialização do retorno da API: {ex.Message}");
                }
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
    }
}
