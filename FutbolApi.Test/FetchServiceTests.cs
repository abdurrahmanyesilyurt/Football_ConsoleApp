using System.Threading.Tasks;
using FutbolApi.Data;
using FutbolApi.Models;
using FutbolApi.Repositories;
using FutbolApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutbolApi.Tests
{
    public class FetchServiceTests
    {
        private readonly FetchService _fetchService;
        private readonly Mock<ILogger<FetchService>> _mockLogger = new Mock<ILogger<FetchService>>();
        private readonly ApplicationDbContext _context= new ApplicationDbContext();
        private readonly Mock<IRepository<League>> _mockLeagueRepository = new Mock<IRepository<League>>();
        private readonly Mock<IRepository<PlayerData>> _mockPlayerRepository = new Mock<IRepository<PlayerData>>();
        private readonly Mock<IRepository<Log>> _mockLogRepository = new Mock<IRepository<Log>>();

        public FetchServiceTests()
        {
            _fetchService = new FetchService(_mockLeagueRepository.Object, _mockPlayerRepository.Object,
                _mockLogRepository.Object, 
                _mockLogger.Object, 
                _context);
        }

        [Fact]
        public async Task ProcessDataAsync_Should_Add_New_Player()
        {
            // Arrange
            var jsonResponse = "{\"data\": [{\"Id\": 1, \"PlayerId\": 1, \"TeamId\": 1, \"PositionId\": 1, \"DetailedPositionId\": 1, \"Captain\": false}]}";
            _mockPlayerRepository.Setup(repo => repo.AddAsync(It.IsAny<PlayerData>())).Returns(Task.CompletedTask);
            _mockPlayerRepository.Setup(repo => repo.GetDb()).Returns(new List<PlayerData>
            {
                new PlayerData { Id = 1,}
            }.AsQueryable()
            );
            // Act
            var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

            // Assert
            _mockPlayerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }


        //[Fact]
        //public async Task ProcessDataAsync_Should_Return_False_When_No_Data()
        //{
        //    // Arrange
        //    var jsonResponse = "{\"data\": []}";

        //    // Act
        //    var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetSportAsync");

        //    // Assert
        //    Assert.False(result);
        //}



        //[Fact]
        //public async Task ProcessDataAsync_Should_Add_New_League()
        //{
        //    // Arrange
        //    var jsonResponse = "{\"data\": {\"Id\": 1, \"SportId\": 1, \"CountryId\": 1, \"Name\": \"League1\", \"Active\": true}}";
        //    _mockLeagueRepository.Setup(repo => repo.AddAsync(It.IsAny<League>())).Returns(Task.CompletedTask);

        //    // Act
        //    var result = await _fetchService.ProcessDataAsync(jsonResponse, "GetLeagueDetailsAsync");

        //    // Assert
        //    _mockLeagueRepository.Verify(repo => repo.AddAsync(It.IsAny<League>()), Times.Once);
        //    _mockLeagueRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        //    Assert.True(result);
        //}


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
    }
}
