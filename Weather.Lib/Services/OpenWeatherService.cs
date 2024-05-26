using RestSharp;
using System.ComponentModel;
using Weather.Lib.Data.Dtos;
using Weather.Lib.Data.Enums;
using Weather.Lib.Services.Interfaces;
using Weather.Lib.Utils;

namespace Weather.Lib.Services
{
    public class OpenWeatherService : IOpenWeatherService
    {
        public static string ApiUrl { get; set; }
        public static string ApiKey { get; set; }
        public static string? Units {  get; set; }

        public async Task<WeatherCurrentDto> GetCurrentWeatherByName(string cityName)
        {
            var queryParams = new List<QueryParameter>();

            queryParams.Add(GetQueryParameter(QueryTypeEnum.CityName, cityName));
            queryParams.Add(GetQueryParameter(QueryTypeEnum.ApiKey, ApiKey));
            if (!string.IsNullOrEmpty(Units))
                queryParams.Add(GetQueryParameter(QueryTypeEnum.Units, Units));

            var response = await RequestHelper.SendAsync<object, OpenWeatherResponseDto>(queryParams, Method.Get, null);
            if (response is null)
                return new WeatherCurrentDto();

            return new WeatherCurrentDto()
            {
                City = cityName,
                Current = new WeatherDto()
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds(response.DateTime).LocalDateTime,
                    Temperature = response.Main.Temperature
                }
            };
        }

        private QueryParameter GetQueryParameter(QueryTypeEnum type, string value)
        {
            switch (type)
            {
                case QueryTypeEnum.ApiKey:
                    return new QueryParameter("appid", value);
                case QueryTypeEnum.CityName:
                    return new QueryParameter("q", value);
                case QueryTypeEnum.Latitude:
                    return new QueryParameter("lat", value);
                case QueryTypeEnum.Longitude:
                    return new QueryParameter("lon", value);
                case QueryTypeEnum.Units:
                    return new QueryParameter("units", value);
                case QueryTypeEnum.Language:
                    return new QueryParameter("lang", value);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
