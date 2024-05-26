using System.ComponentModel.DataAnnotations;

namespace Weather.Lib.Data.Commands
{
    public class CustomWeatherCommand
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}
