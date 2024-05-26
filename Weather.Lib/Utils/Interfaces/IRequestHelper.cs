using RestSharp;

namespace Weather.Lib.Utils.Interfaces
{
    public interface IRequestHelper
    {
        Task<R?> SendAsync<T, R>(List<QueryParameter> queryParams, Method method, T command);
    }
}