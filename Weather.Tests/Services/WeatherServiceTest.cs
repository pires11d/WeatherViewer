using Moq;
using Weather.Lib.Data.Commands;
using Weather.Lib.Data.Dtos;
using Weather.Lib.Services;
using Weather.Lib.Services.Interfaces;
using Weather.Lib.Utils.Interfaces;

namespace Weather.Tests.Services
{
    [TestClass]
    public class WeatherServiceTest
    {
        private WeatherService _service;
        private Mock<IOpenWeatherService> _openWeatherServiceMock;
        private Mock<IFileHelper> _fileHelperMock;
        private WeatherCurrentDto _mockWeather1;
        private WeatherCurrentDto _mockWeather2;
        private WeatherCurrentDto _mockWeather3;

        [TestInitialize]
        public void Setup()
        {
            _fileHelperMock = new Mock<IFileHelper>();
            _openWeatherServiceMock = new Mock<IOpenWeatherService>();
            _service = new WeatherService(
                _openWeatherServiceMock.Object,
                _fileHelperMock.Object
            );

            _mockWeather1 = new WeatherCurrentDto
            {
                City = "Curitiba",
                Current = new WeatherDto
                {
                    Time = DateTime.Now,
                    Temperature = 10.2m
                }
            };
            _mockWeather2 = new WeatherCurrentDto
            {
                City = "Florianópolis",
                Current = new WeatherDto
                {
                    Time = DateTime.Now,
                    Temperature = 13.5m
                }
            };
            _mockWeather3 = new WeatherCurrentDto
            {
                City = "Porto Alegre",
                Current = new WeatherDto
                {
                    Time = DateTime.Now,
                    Temperature = 8.3m
                }
            };
        }

        [TestMethod]
        public async Task ProcessSchedule_ValidConfiguration_Should_Call_GetCurrent_And_SaveCurrent()
        {
            // Arrange
            WeatherService.Cities = new List<string> { "Curitiba", "Florianópolis", "Porto Alegre" };
            _openWeatherServiceMock.SetupSequence(x => x.GetCurrentWeatherByName(It.IsAny<string>()))
                                   .ReturnsAsync(_mockWeather1)
                                   .ReturnsAsync(_mockWeather2)
                                   .ReturnsAsync(_mockWeather3);

            _fileHelperMock.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()));

            // Act
            await _service.ProcessSchedule();

            // Assert
            _openWeatherServiceMock.Verify(x => x.GetCurrentWeatherByName(It.IsAny<string>()), Times.Exactly(3));
            _fileHelperMock.Verify(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task ProcessSchedule_InvalidConfiguration_Should_Call_GetCurrent_And_NotSaveCurrent()
        {
            // Arrange
            WeatherService.Cities = new List<string> { "" };
            _openWeatherServiceMock.Setup(x => x.GetCurrentWeatherByName(It.IsAny<string>()))
                                   .ReturnsAsync((WeatherCurrentDto)null);

            // Act
            await _service.ProcessSchedule();

            // Assert
            _openWeatherServiceMock.Verify(x => x.GetCurrentWeatherByName(It.IsAny<string>()), Times.Once);
            _fileHelperMock.Verify(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [TestMethod]
        public void GetCurrent_Should_Return_List_Of_WeatherCurrentDto()
        {
            // Arrange
            WeatherService.Cities = new List<string> { "Curitiba", "Florianópolis", "Porto Alegre" };
            var expectedResults = new List<WeatherCurrentDto> { _mockWeather1, _mockWeather2, _mockWeather3 };

            _openWeatherServiceMock.SetupSequence(x => x.GetCurrentWeatherByName(It.IsAny<string>()))
                                   .ReturnsAsync(_mockWeather1)
                                   .ReturnsAsync(_mockWeather2)
                                   .ReturnsAsync(_mockWeather3);

            // Act
            var result = _service.GetCurrent();

            // Assert
            CollectionAssert.AreEqual(expectedResults.Select(x => x.City).ToList(), result.Select(x => x.City).ToList());
            CollectionAssert.AreEqual(expectedResults.Select(x => x.Current.Temperature).ToList(), result.Select(x => x.Current.Temperature).ToList());
        }

        [TestMethod]
        public void GetHistory_Should_Return_List_Of_WeatherHistoryDto()
        {
            // Arrange
            WeatherService.Cities = new List<string> { "Curitiba", "Florianópolis", "Porto Alegre" };
            var command = new WeatherCommand
            {
                Cities = new List<string> { "Curitiba", "Florianópolis", "Porto Alegre" },
                StartDate = new DateOnly(2024, 5, 25),
                EndDate = new DateOnly(2024, 5, 25)
            };

            _fileHelperMock.SetupSequence(x => x.GetFiles(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                           .Returns(new List<string> { "2024-05-25,10.2" })
                           .Returns(new List<string> { "2024-05-25,13.5" })
                           .Returns(new List<string> { "2024-05-25,8.3" });

            // Act
            var result = _service.GetHistory(command);

            // Assert
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(command.Cities, result.Select(x => x.City).ToList());
            CollectionAssert.AreEqual(new List<decimal> { 10.2m, 13.5m, 8.3m }, result.Select(x => x.History.First().Temperature).ToList());
        }
    }
}
