using Weather.Lib.Data.Dtos;

namespace Weather.Lib.Services.Interfaces
{
    public interface IOpenWeatherService
    {
        Task<WeatherCurrentDto> GetCurrentWeatherByName(string cityName);
    }
}
