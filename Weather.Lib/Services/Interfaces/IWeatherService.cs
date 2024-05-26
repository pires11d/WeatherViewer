using Weather.Lib.Data.Commands;
using Weather.Lib.Data.Dtos;

namespace Weather.Lib.Services.Interfaces
{
    public interface IWeatherService
    {
        List<WeatherCurrentDto> GetCurrent();
        List<WeatherHistoryDto> GetHistory(WeatherCommand command);
        Task ProcessSchedule();
    }
}
