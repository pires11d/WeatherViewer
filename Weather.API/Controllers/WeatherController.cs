using Microsoft.AspNetCore.Mvc;
using System.Net;
using Weather.Lib.Data.Commands;
using Weather.Lib.Services.Interfaces;

namespace Weather.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas chamadas referentes ao tempo
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _service;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public WeatherController(ILogger<WeatherController> logger, IWeatherService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Busca as temperaturas atuais das cidades configuradas por default na API
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrent")]
        public IActionResult GetCurrent()
        {
            try
            {
                var result = _service.GetCurrent();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca as temperaturas de uma ou mais cidades, em um período de tempo
        /// </summary>
        /// <remarks>
        /// Insira uma ou mais cidades para consultar, no corpo da requisição:
        /// <br></br>
        /// Ex.: Curitiba, Florianópolis, Porto Alegre
        /// </remarks>
        /// <returns></returns>
        [HttpPost("GetHistory")]
        public IActionResult GetHistory([FromBody] WeatherCommand request)
        {
            try
            {
                var result = _service.GetHistory(request);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}
