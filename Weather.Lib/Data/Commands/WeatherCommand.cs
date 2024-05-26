using System.ComponentModel.DataAnnotations;

namespace Weather.Lib.Data.Commands
{
    public class WeatherCommand
    {
        [Required]
        public List<string> Cities { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
