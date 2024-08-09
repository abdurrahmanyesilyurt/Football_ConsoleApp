using System.Threading.Tasks;
using FutbolApi.Models;
using FutbolApi.Repositories;
using FutbolApi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutbolApi.Tests
{
    public class FetchServiceTests
    {
        private readonly FetchService _fetchService;
        private readonly Mock<ILogger<FetchService>> _mockLogger = new Mock<ILogger<FetchService>>();
        private readonly Mock<IRepository<League>> _mockLeagueRepository = new Mock<IRepository<League>>();
        private readonly Mock<IRepository<PlayerData>> _mockPlayerRepository = new Mock<IRepository<PlayerData>>();
        private readonly Mock<IRepository<Log>> _mockLogRepository = new Mock<IRepository<Log>>();

        public FetchServiceTests()
        {
            _fetchService = new FetchService(
                _mockLeagueRepository.Object,
                _mockPlayerRepository.Object,
                _mockLogRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Add_New_Player()
        {
            // Arrange
            var jsonResponse = "{\"data\": [{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false}]}";
            _mockPlayerRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>().AsQueryable());

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert
            _mockPlayerRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>()), Times.Once);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Not_Add_Existing_Player()
        {
            // Arrange
            var jsonResponse = "{\"data\": [{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false, \"JerseyNumber\": 10}]}";

            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>
    {
        new PlayerData { Id = 1, PlayerId = 1, JerseyNumber = 10 }
    }.AsQueryable());

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert
            _mockPlayerRepository.Verify(repo => repo.AddAsync(It.IsAny<PlayerData>()), Times.Never);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Add_Multiple_Players()
        {
            // Arrange
            var jsonResponse = "{\"data\": [{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false}, {\"Id\": 2, \"PlayerId\": 2, \"TeamId\": 1, \"PositionId\": 2, \"DetailedPositionId\": 2, \"Captain\": false}]}";
            _mockPlayerRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>().AsQueryable());

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert
            _mockPlayerRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Add_New_League()
        {
            // Arrange
            var jsonResponse = "{\"data\": {\"Id\": 1, \"SportId\": 1, \"CountryId\": 1, \"Name\": \"League1\", \"Active\": true}}";
            _mockLeagueRepository.Setup(repo => repo.AddAsync(It.IsAny<League>())).Returns(Task.CompletedTask);
            _mockLeagueRepository.Setup(repo => repo.GetDb()).Returns(new List<League>().AsQueryable());

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetLeagueDetailsAsync");

            // Assert
            _mockLeagueRepository.Verify(repo => repo.AddAsync(It.IsAny<League>()), Times.Once);
            _mockLeagueRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Return_False_When_No_Data()
        {
            // Arrange
            var jsonResponse = "{\"data\": []}";

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert
            Assert.False(result);
        }
[Fact]
public async Task ProcessDataAsync_Should_Add_Log_On_Error()
{
    // Arrange
    var jsonResponse = "{\"data\": {\"Id\": 1, \"SportId\": 1, \"CountryId\": 1, \"Name\": \"League1\", \"Active\": true}}";

    // Simulate a failure in adding a league to trigger the exception
    _mockLeagueRepository.Setup(repo => repo.AddAsync(It.IsAny<League>())).ThrowsAsync(new Exception("Database error"));

    // Act
    var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetLeagueDetailsAsync");

    // Assert
    _mockLogRepository.Verify(repo => repo.AddAsync(It.IsAny<Log>()), Times.Once);
    Assert.False(result);
}

        [Fact]
        public async Task ProcessDataAsync_Should_Handle_Missing_League_Fields()
        {
            // Arrange
            var jsonResponse = "{\"data\": {\"Id\": 1, \"Name\": \"League1\"}}"; // Missing SportId and CountryId
            _mockLeagueRepository.Setup(repo => repo.AddAsync(It.IsAny<League>())).Returns(Task.CompletedTask);

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetLeagueDetailsAsync");

            // Assert
            _mockLeagueRepository.Verify(repo => repo.AddAsync(It.IsAny<League>()), Times.Once);
            _mockLeagueRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Return_False_On_Invalid_Json()
        {
            // Arrange
            var invalidJsonResponse = "{\"invalid\": \"data\"}";

            // Act
            var result = await _fetchService.ProcessDataAsync(invalidJsonResponse, "GetSportAsync");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Return_False_When_League_Detail_Is_Null()
        {
            // Arrange
            var jsonResponse = "{\"data\": null}";

            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetLeagueDetailsAsync");

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Player()
        {
            // Arrange
            var jsonResponse = "{\"data\": [{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false}]}";

            // İlk ekleme için setup
            _mockPlayerRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>().AsQueryable());
            _mockPlayerRepository.Setup(repo => repo.Remove(It.IsAny<PlayerData>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act - İlk ekleme
            var addResult1 = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert - İlk ekleme başarılı olmalı
            _mockPlayerRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>()), Times.Once);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(addResult1);

            // Act - Silme işlemi
            var playerToRemove = new PlayerData { Id = 1, PlayerId = 1, TeamId = 1, PositionId = 1, DetailedPositionId = 1, Captain = false };
            await _mockPlayerRepository.Object.Remove(playerToRemove);
            await _mockPlayerRepository.Object.SaveChangesAsync();

            // Verify - Silme işleminin gerçekleştiğinden emin olun
            _mockPlayerRepository.Verify(repo => repo.Remove(It.IsAny<PlayerData>()), Times.Once);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(2));

            // Act - Tekrar ekleme
            var addResult2 = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert - Tekrar ekleme başarılı olmalı
            _mockPlayerRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>()), Times.Exactly(2));
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(3));
            Assert.True(addResult2);
        }
        [Fact]
        public async Task ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Multiple_Players()
        {
            // Arrange
            var jsonResponse = "{\"data\": [" +
                "{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false}," +
                "{\"Id\": 2, \"PlayerId\": 2, \"TeamId\": 2, \"PositionId\": 2, \"DetailedPositionId\": 2, \"Captain\": true}" +
                "]}";

            var playersToAdd = new List<PlayerData>
    {
        new PlayerData { Id = 1, PlayerId = 1, TeamId = 1, PositionId = 1, DetailedPositionId = 1, Captain = false },
        new PlayerData { Id = 2, PlayerId = 2, TeamId = 2, PositionId = 2, DetailedPositionId = 2, Captain = true }
    };

            _mockPlayerRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<PlayerData>>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>().AsQueryable());
            _mockPlayerRepository.Setup(repo => repo.Remove(It.IsAny<PlayerData>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act - İlk ekleme
            var addResult1 = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert - İlk ekleme başarılı olmalı
            _mockPlayerRepository.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<PlayerData>>(players => players.Count() == 2)), Times.Once);
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(addResult1);

            // Act - Silme işlemi
            foreach (var player in playersToAdd)
            {
                await _mockPlayerRepository.Object.Remove(player);
            }
            await _mockPlayerRepository.Object.SaveChangesAsync();

            // Verify - Silme işleminin gerçekleştiğinden emin olun
            _mockPlayerRepository.Verify(repo => repo.Remove(It.IsAny<PlayerData>()), Times.Exactly(2));
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(2));

            // Act - Tekrar ekleme
            var addResult2 = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert - Tekrar ekleme başarılı olmalı
            _mockPlayerRepository.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<PlayerData>>(players => players.Count() == 2)), Times.Exactly(2));
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(3));
            Assert.True(addResult2);
        }


    }
}
