using System.Text.Json.Serialization;

namespace Weather.Lib.Data.Dtos
{
    public class OpenWeatherResponseDto
    {
        [JsonPropertyName("coord")]
        public CoordinatesDto? Coordinates { get; set; }

        [JsonPropertyName("main")]
        public MainDto? Main { get; set; }

        [JsonPropertyName("dt")]
        public long DateTime { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class CoordinatesDto
    {
        [JsonPropertyName("lon")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("lat")]
        public decimal Latitude { get; set; }
    }

    public class MainDto
    {
        [JsonPropertyName("temp")]
        public decimal Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double TemperatureMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double TemperatureMax { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }
}
