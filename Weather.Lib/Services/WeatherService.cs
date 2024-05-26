using System.Globalization;
using Weather.Lib.Data.Commands;
using Weather.Lib.Data.Dtos;
using Weather.Lib.Services.Interfaces;
using Weather.Lib.Utils;

namespace Weather.Lib.Services
{
    public class WeatherService : IWeatherService
    {
        public static List<string> Cities { get; set; }

        private readonly IOpenWeatherService _openWeatherService;

        public WeatherService(IOpenWeatherService openWeatherService)
        {
            _openWeatherService = openWeatherService;
        }

        public List<WeatherCurrentDto> GetCurrent()
        {
            var result = new List<WeatherCurrentDto>();
            var tasks = new List<Task<WeatherCurrentDto>>();

            foreach (var city in Cities.Distinct())
            {
                tasks.Add(_openWeatherService.GetCurrentWeatherByName(city));
            }

            Task.WaitAll(tasks.ToArray());

            result = tasks.Select(task => task.Result).ToList();

            return result;
        }

        public List<WeatherHistoryDto> GetHistory(WeatherCommand command)
        {
            var result = new List<WeatherHistoryDto>();

            foreach (var city in command.Cities.Distinct())
            {
                var history = GetHistoryFromFiles(city, command.StartDate, command.EndDate);

                result.Add(new WeatherHistoryDto()
                {
                    City = city,
                    History = history
                });
            }

            return result;
        }

        public List<WeatherHistoryDto> GetDefaultHistory()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var defaultCommand = new WeatherCommand(Cities, today, today);

            var result = GetHistory(defaultCommand);

            return result;
        }

        private List<WeatherDto> GetHistoryFromFiles(string city, DateOnly startDate, DateOnly endDate)
        {
            var result = new List<WeatherDto>();

            var entries = FileHelper.GetFiles(city, startDate, endDate);

            foreach (var entry in entries)
            {
                var time = entry.Split(",").FirstOrDefault();
                var temperature = entry.Split(",").LastOrDefault();
                if (!string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(temperature))
                {
                    result.Add(new WeatherDto()
                    {
                        Time = Convert.ToDateTime(time, CultureInfo.InvariantCulture),
                        Temperature = Convert.ToDecimal(temperature, CultureInfo.InvariantCulture)
                    });
                }
            }

            return result.OrderByDescending(x => x.Time).ToList();
        }

        public Task ProcessSchedule()
        {
            var result = GetCurrent();

            SaveCurrent(result);

            return Task.CompletedTask;
        }

        private void SaveCurrent(List<WeatherCurrentDto> results)
        {
            try
            {
                var now = DateTime.Now;
                foreach (var result in results)
                {
                    var folderName = result.City;
                    var fileName = now.ToString("yyyy-MM-dd");
                    var content = new string[] { now.ToString("s"), result.Current.Temperature.ToString("F2", CultureInfo.InvariantCulture) };
                    FileHelper.SaveFile(folderName, fileName, content);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ocorreu um erro ao salvar o arquivo texto: {ex.Message}");
            }
        }
    }
}
