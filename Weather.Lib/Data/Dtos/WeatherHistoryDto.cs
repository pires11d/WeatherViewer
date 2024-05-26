namespace Weather.Lib.Data.Dtos
{
    public class WeatherHistoryDto
    {
        public string? City { get; set; }

        public List<WeatherDto> History { get; set; } = new List<WeatherDto>();
    }
}
