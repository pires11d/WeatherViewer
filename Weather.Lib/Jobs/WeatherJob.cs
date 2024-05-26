using Quartz;
using Weather.Lib.Services.Interfaces;

namespace Weather.Lib.Jobs
{
    public class WeatherJob : IJob
    {
        private readonly IWeatherService _weatherService;

        public WeatherJob(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.FromResult(_weatherService.ProcessSchedule());
        }
    }
}
