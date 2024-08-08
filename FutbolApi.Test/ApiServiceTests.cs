using System.Net.Http;
using System.Threading.Tasks;
using FutbolApi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutbolApi.Tests
{
    public class ApiServiceTests
    {
        private readonly ApiService _apiService;
        private readonly Mock<IFetchDataService> _mockFetchDataService = new Mock<IFetchDataService>();

        private readonly Mock<ILogger<ApiService>> _mockLogger = new Mock<ILogger<ApiService>>();


        public ApiServiceTests()
        {
            _apiService = new ApiService(
                "fake_token",
                _mockFetchDataService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetSportAsync_Should_Call_FetchDataService_FetchDataAsync()
        {
            // Act
            await _apiService.GetSportAsync();

            // Assert
            _mockFetchDataService.Verify(x => x.FetchDataAsync(It.IsAny<string>(), "GetSportAsync"), Times.Once);
        }

        [Fact]
        public async Task GetLeaguesAsync_Should_Call_FetchDataService_FetchDataAsync()
        {
            // Act
            await _apiService.GetLeaguesAsync();

            // Assert
            _mockFetchDataService.Verify(x => x.FetchDataAsync(It.IsAny<string>(), "GetLeaguesAsync"), Times.Once);
        }

        [Fact]
        public async Task GetLeagueDetailsAsync_Should_Call_FetchDataService_FetchDataAsync()
        {
            // Arrange
            string leagueId = "123";

            // Act
            await _apiService.GetLeagueDetailsAsync(leagueId);

            // Assert
            _mockFetchDataService.Verify(x => x.FetchDataAsync(It.IsAny<string>(), "GetLeagueDetailsAsync"), Times.Once);
        }
    }
}
