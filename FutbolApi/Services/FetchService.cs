using System;
using System.Threading.Tasks;
using FutbolApi.Models;
using FutbolApi.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FutbolApi.Data;

namespace FutbolApi.Services
{
    public class FetchService
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<PlayerData> _playerRepository;
        private readonly IRepository<Log> _logRepository;
        private readonly ILogger<FetchService> _logger;

        public FetchService(IRepository<League> leagueRepository, IRepository<PlayerData> playerRepository, IRepository<Log> logRepository, ILogger<FetchService> logger, ApplicationDbContext context)
        {
            _leagueRepository = leagueRepository;
            _playerRepository = playerRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<bool> ProcessDataAsync(string responseBody, string operationName)
        {
            if (operationName == "GetLeagueDetailsAsync")
            {
                LeagueDetailApiResponse leagueDetailResponse = JsonConvert.DeserializeObject<LeagueDetailApiResponse>(responseBody);
                if (leagueDetailResponse?.Data == null)
                {
                    _logger.LogError("No valid data found in the response for {Operation}", operationName);
                    return false;
                }

                League league = leagueDetailResponse.Data;

                League existingLeague = await _leagueRepository.GetByIdAsync(league.Id);

                if (existingLeague != null)
                {
                    existingLeague.Name = league.Name;
                }
                else
                {
                    await _leagueRepository.AddAsync(league);
                }

                await _leagueRepository.SaveChangesAsync();

                await LogToDatabase("Info", operationName, "Fetched data from API successfully");

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
            else if (operationName == "GetLeaguesAsync")
            {
                ApiResponse<League> apiResponse = JsonConvert.DeserializeObject<ApiResponse<League>>(responseBody);
                if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                {
                    _logger.LogError("No valid data found in the response for {Operation}", operationName);
                    return false;
                }

                HashSet<int> id_League = apiResponse.Data.Select(s => s.Id).ToHashSet();

                List<League> local_league = _leagueRepository.GetDb().Where(d => id_League.Contains(d.Id)).ToList();

                foreach (League league in apiResponse.Data)
                {
                    League updatedLeagueData = local_league.SingleOrDefault(a => a.Id == league.Id);

                    if (updatedLeagueData is not null)
                    {
                        updatedLeagueData.Name = league.Name;
                    }
                    else
                    {
                        await _leagueRepository.AddAsync(league);
                    }
                }
                await _leagueRepository.SaveChangesAsync();

                await LogToDatabase("Info", operationName, "Fetched data from API successfully");


                local_league = (await _leagueRepository.GetAllAsync()).ToList();


                foreach (League league in local_league)
                {
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
                }

                return true;

            }
            else
            {
                ApiResponse<PlayerData> apiResponse = JsonConvert.DeserializeObject<ApiResponse<PlayerData>>(responseBody);
                if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                {
                    _logger.LogError("No valid data found in the response for {Operation}", operationName);
                    return false;
                }

                HashSet<int> ids = apiResponse.Data.Select(s => s.Id).ToHashSet();

                List<PlayerData> local = _playerRepository.GetDb().Where(d => ids.Contains(d.Id)).ToList();

                foreach (PlayerData player in apiResponse.Data)
                {
                    PlayerData updatedDate = local.SingleOrDefault(a => a.Id == player.Id);

                    if (updatedDate is not null)
                    {
                        updatedDate.JerseyNumber = player.JerseyNumber;
                    }
                    else
                    {
                        await _playerRepository.AddAsync(player);
                    }
                }
                await _playerRepository.SaveChangesAsync();

                await LogToDatabase("Info", operationName, "Fetched data from API successfully");


                local = (await _playerRepository.GetAllAsync()).ToList();


                foreach (PlayerData player in local)
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

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
        }
    }
}
