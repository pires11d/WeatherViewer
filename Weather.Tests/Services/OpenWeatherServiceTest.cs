using Moq;
using RestSharp;
using Weather.Lib.Data.Dtos;
using Weather.Lib.Services;
using Weather.Lib.Utils.Interfaces;

namespace Weather.Tests.Services
{
    [TestClass]
    public class OpenWeatherServiceTest
    {
        private OpenWeatherService _service;
        private Mock<IRequestHelper> _requestHelperMock;

        [TestInitialize]
        public void Setup()
        {
            _requestHelperMock = new Mock<IRequestHelper>();
            _service = new OpenWeatherService(_requestHelperMock.Object);
        }

        [TestMethod]
        public async Task GetCurrentWeatherByName_ValidCity_ReturnsDto()
        {
            // Arrange
            var cityName = "Lages";
            var responseDto = new OpenWeatherResponseDto
            {
                Name = cityName,
                DateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Main = new MainDto() 
                { 
                    Temperature = 2 
                }
            };

            _requestHelperMock.Setup(x => x.SendAsync<object, OpenWeatherResponseDto>(It.IsAny<List<QueryParameter>>(), Method.Get, null))
                              .ReturnsAsync(responseDto);
            // Act
            var weatherCurrent = await _service.GetCurrentWeatherByName(cityName);

            // Assert
            Assert.IsNotNull(weatherCurrent);
            Assert.AreEqual(cityName, weatherCurrent.City);
            Assert.IsNotNull(weatherCurrent.Current);
            Assert.IsNotNull(weatherCurrent.Current.Time);
            Assert.IsNotNull(weatherCurrent.Current.Temperature);
        }

        [TestMethod]
        public async Task GetCurrentWeatherByName_InvalidCity_ReturnsEmptyDto()
        {
            // Arrange
            var cityName = "Jarbas";

            _requestHelperMock.Setup(x => x.SendAsync<object, OpenWeatherResponseDto>(It.IsAny<List<QueryParameter>>(), Method.Get, null))
                              .ReturnsAsync((OpenWeatherResponseDto)null);

            // Act
            var weatherCurrent = await _service.GetCurrentWeatherByName(cityName);

            // Assert
            Assert.IsNotNull(weatherCurrent);
            Assert.IsNull(weatherCurrent.City);
            Assert.IsNull(weatherCurrent.Current);
        }
    }
}