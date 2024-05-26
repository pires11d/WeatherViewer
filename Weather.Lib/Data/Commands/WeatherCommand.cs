using System.ComponentModel.DataAnnotations;

namespace Weather.Lib.Data.Commands
{
    public class WeatherCommand
    {
        public WeatherCommand()
        {
        }

        public WeatherCommand(List<string> cities, string startDate, string endDate)
        {
            try
            {
                Cities = cities;
                StartDate = DateOnly.Parse(startDate);
                EndDate = DateOnly.Parse(endDate);
            }
            catch (Exception)
            {
                throw new ApplicationException("Parâmetros inválidos!");
            }
        }

        public WeatherCommand(List<string> cities, DateOnly startDate, DateOnly endDate)
        {
            Cities = cities;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Lista de cidades
        /// </summary>
        /// <remarks>
        /// Ex.: ["Curitiba", "Florianópolis", "Porto Alegre"]
        /// </remarks>
        [Required]
        public List<string> Cities { get; set; } = new List<string>();

        /// <summary>
        /// Data inicial
        /// </summary>
        /// <remarks>
        /// Ex.: "2024-05-25"
        /// </remarks>
        [Required]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Data final
        /// </summary>
        /// <remarks>
        /// Ex.: "2024-05-26"
        /// </remarks>
        [Required]
        public DateOnly EndDate { get; set; }
    }
}
