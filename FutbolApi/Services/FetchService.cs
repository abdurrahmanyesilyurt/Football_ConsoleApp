using System;
using System.Threading.Tasks;
using FutbolApi.Data;
using FutbolApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FutbolApi.Services
{
    public class FetchService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<FetchService> logger;

        public FetchService(ApplicationDbContext dbContext, ILogger<FetchService> logger)
        {
            context = dbContext;
            this.logger = logger;
        }

        public async Task<bool> ProcessDataAsync(string responseBody, string operationName)
        {
            if (operationName == "GetLeagueDetailsAsync")
            {
                var leagueDetailResponse = JsonConvert.DeserializeObject<LeagueDetailApiResponse>(responseBody);
                if (leagueDetailResponse?.Data == null)
                {
                    logger.LogError("No valid data found in the response for {Operation}", operationName);
                    return false;
                }

                var league = leagueDetailResponse.Data;
                var existingLeague = await context.Leagues.FindAsync(league.Id);
                if (existingLeague != null)
                {
                    // Update existing league
                    context.Entry(existingLeague).CurrentValues.SetValues(league);
                }
                else
                {
                    // Add new league
                    context.Leagues.Add(league);
                }

                await context.SaveChangesAsync();

                // Log the operation to the database
                await LogToDatabase("Info", operationName, "Fetched data from API successfully");

                // Display the data in a readable format
                Console.WriteLine($"Id: {league.Id},\n" +
                                  $"SportId: {CheckNull(league.SportId)},\n" +
                                  $"CountryId: {CheckNull(league.CountryId)},\n" +
                                  $"Name: {CheckNull(league.Name)},\n" +
                                  $"Active: {CheckNull(league.Active)},\n" +
                                  $"ShortCode: {CheckNull(league.ShortCode)},\n" +
                                  $"ImagePath: {CheckNull(league.ImagePath)},\n" +
                                  $"Type: {CheckNull(league.Type)},\n" +
                                  $"SubType: {CheckNull(league.SubType)},\n" +
                                  $"LastPlayedAt: {CheckNull(league.LastPlayedAt)},\n" +
                                  $"Category: {CheckNull(league.Category)},\n" +
                                  $"HasJerseys: {CheckNull(league.HasJerseys)}\n");

                return true;
            }
            else
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                if (apiResponse?.Data == null)
                {
                    logger.LogError("No valid data found in the response for {Operation}", operationName);
                    return false;
                }

                foreach (var player in apiResponse.Data)
                {
                    var existingPlayer = await context.Players.FindAsync(player.Id);
                    if (existingPlayer != null)
                    {
                        context.Entry(existingPlayer).CurrentValues.SetValues(player);
                    }
                    else
                    {
                        context.Players.Add(player);
                    }
                }

                await context.SaveChangesAsync();

                await LogToDatabase("Info", operationName, "Fetched data from API successfully");

                foreach (var player in apiResponse.Data)
                {
                    Console.WriteLine($"Id: {player.Id},\n" +
                                      $"TransferId: {CheckNull(player.TransferId)},\n" +
                                      $"PlayerId: {CheckNull(player.PlayerId)},\n" +
                                      $"TeamId: {CheckNull(player.TeamId)}\n" +
                                      $"PositionId: {CheckNull(player.PositionId)},\n" +
                                      $"DetailedPositionId: {CheckNull(player.DetailedPositionId)},\n" +
                                      $"Start: {CheckNull(player.Start)},\n" +
                                      $"End: {CheckNull(player.End)},\n" +
                                      $"Captain: {CheckNull(player.Captain)},\n" +
                                      $"JerseyNumber: {CheckNull(player.JerseyNumber)}\n");
                }

                return true;
            }
        }
        //Null değerleri boş döndürmeceler
        private string CheckNull(object value)
        {
            return value == null ? "boş" : value.ToString();
        }

        public async Task LogToDatabase(string level, string logger, string message, string exception = null)
        {
            var log = new Log
            {
                Date = DateTime.UtcNow,
                Level = level,
                Logger = logger,
                Message = message,
                Exception = exception ?? string.Empty 
            };

            context.Logs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}
