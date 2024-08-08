using System;
using System.Net.Http;
using System.Threading.Tasks;
using FutbolApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FutbolApi.Services
{
    public class FetchDataService : IFetchDataService
    {
        private readonly FetchService fetchService;
        private readonly ILogger<FetchDataService> logger;

        public FetchDataService(FetchService fetchService, ILogger<FetchDataService> logger)
        {
            this.fetchService = fetchService;
            this.logger = logger;
        }

        public async Task FetchDataAsync(string url, string operationName)
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
