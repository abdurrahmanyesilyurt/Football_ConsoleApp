using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FutbolApi.Services
{
    public class ApiService
    {
        private readonly string _token;
        private readonly IFetchDataService _fetchDataService;
        private readonly ILogger<ApiService> _logger;

        public ApiService(string apiToken, IFetchDataService fetchDataService, ILogger<ApiService> logger)
        {
            _token = apiToken;
            _fetchDataService = fetchDataService;
            _logger = logger;
        }

        public async Task GetSportAsync()
        {
            string url = $"https://api.sportmonks.com/v3/football/squads/teams/1?api_token={_token}";
            await _fetchDataService.FetchDataAsync(url, "GetSportAsync");
        }

        public async Task GetLeaguesAsync()
        {
            string url = $"https://api.sportmonks.com/v3/football/leagues?api_token={_token}";
            await _fetchDataService.FetchDataAsync(url, "GetLeaguesAsync");
        }

        public async Task GetLeagueDetailsAsync(string leagueId)
        {
            string url = $"https://api.sportmonks.com/v3/football/leagues/{leagueId}?api_token={_token}";
            await _fetchDataService.FetchDataAsync(url, "GetLeagueDetailsAsync");
        }
    }
}
