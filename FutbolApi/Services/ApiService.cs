using System;
using System.Net.Http;
using System.Threading.Tasks;
using FutbolApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FutbolApi.Services
{
    public class ApiService
    {
        private readonly string token;
        private readonly FetchService fetchService;
        private readonly ILogger<ApiService> logger;

        public ApiService(string apiToken, FetchService fetchService, ILogger<ApiService> logger)
        {
            token = apiToken;
            this.fetchService = fetchService;
            this.logger = logger;
        }

        public async Task GetSportAsync()
        {
            string url = $"https://api.sportmonks.com/v3/football/squads/teams/1?api_token={token}";
            await FetchDataAsync(url, "GetSportAsync");
        }

        public async Task GetLeaguesAsync()
        {
            string url = $"https://api.sportmonks.com/v3/football/leagues?api_token={token}";
            await FetchDataAsync(url, "GetLeaguesAsync");
        }

        public async Task GetLeagueDetailsAsync(string leagueId)
        {
            string url = $"https://api.sportmonks.com/v3/football/leagues/{leagueId}?api_token={token}";
            await FetchDataAsync(url, "GetLeagueDetailsAsync");
        }

        private async Task FetchDataAsync(string url, string operationName)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(responseBody))
                    {
                        Console.WriteLine("No data returned from the API.");
                        await fetchService.LogToDatabase("Error", operationName, "No data returned from the API.");
                        return;
                    }

                    // Call FetchService to process the data
                    bool isDataProcessed = await fetchService.ProcessDataAsync(responseBody, operationName);

                    if (!isDataProcessed)
                    {
                        Console.WriteLine("The data was invalid or not found.");
                        await fetchService.LogToDatabase("Error", operationName, "The data was invalid or not found.");
                        return;
                    }

                    logger.LogInformation("Fetched data from API: {Response}", responseBody);
                }
                catch (HttpRequestException e)
                {
                    logger.LogError(e, "An error occurred while fetching data from the API.");
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);

                    await fetchService.LogToDatabase("Error", operationName, e.Message, e.ToString());
                }
                catch (DbUpdateException e)
                {
                    logger.LogError(e, "An error occurred while updating the database.");
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);

                    await fetchService.LogToDatabase("Error", operationName, e.Message, e.ToString());
                }
            }
        }
    }
}
