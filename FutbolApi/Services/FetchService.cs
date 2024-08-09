using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutbolApi.Models;
using FutbolApi.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FutbolApi.Services
{
    public class FetchService
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<PlayerData> _playerRepository;
        private readonly IRepository<Log> _logRepository;
        private readonly ILogger<FetchService> _logger;

        public FetchService(
            IRepository<League> leagueRepository,
            IRepository<PlayerData> playerRepository,
            IRepository<Log> logRepository,
            ILogger<FetchService> logger)
        {
            _leagueRepository = leagueRepository;
            _playerRepository = playerRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<bool> ProcessDataAsync(string responseBody, string operationName)
        {
            try
            {
                if (operationName == "GetLeagueDetailsAsync")
                {
                    var leagueDetailResponse = JsonConvert.DeserializeObject<LeagueDetailApiResponse>(responseBody);
                    if (leagueDetailResponse?.Data == null)
                    {
                        _logger.LogError("No valid data found in the response for {Operation}", operationName);
                        return false;
                    }

                    var league = leagueDetailResponse.Data;
                    var existingLeague = await _leagueRepository.GetByIdAsync(league.Id);

                    if (existingLeague != null)
                    {
                        existingLeague.CountryId = league.CountryId;
                    }
                    else
                    {
                        await _leagueRepository.AddAsync(league);
                    }

                    await _leagueRepository.SaveChangesAsync();
                    await LogToDatabase("Info", operationName, "Fetched data from API successfully");

                    // Fetch and print the league from the database after the update
                    var updatedLeague = await _leagueRepository.GetByIdAsync(league.Id);
                    if (updatedLeague != null)
                    {
                        PrintLeagueData(updatedLeague);
                    }
                    return true;
                }
                else if (operationName == "GetLeaguesAsync")
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<League>>(responseBody);
                    if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                    {
                        _logger.LogError("No valid data found in the response for {Operation}", operationName);
                        return false;
                    }

                    var idLeagueSet = apiResponse.Data.Select(s => s.Id).ToHashSet();
                    var localLeagues = _leagueRepository.GetDb().Where(d => idLeagueSet.Contains(d.Id)).ToList();

                    var newLeagues = new List<League>();

                    foreach (var league in apiResponse.Data)
                    {
                        var existingLeague = localLeagues.SingleOrDefault(a => a.Id == league.Id);

                        if (existingLeague != null)
                        {
                            existingLeague.Name = league.Name;
                        }
                        else
                        {
                            newLeagues.Add(league); // Yeni ligleri toplu olarak eklemek için listeye ekle
                        }
                    }

                    if (newLeagues.Any())
                    {
                        await _leagueRepository.AddRangeAsync(newLeagues); // Tüm yeni ligleri toplu olarak ekle
                    }

                    await _leagueRepository.SaveChangesAsync(); // Değişiklikleri tek seferde kaydet
                    await LogToDatabase("Info", operationName, "Fetched data from API successfully");

                    PrintAllLeagues();
                    return true;
                }
                else
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PlayerData>>(responseBody);
                    if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                    {
                        _logger.LogError("No valid data found in the response for {Operation}", operationName);
                        return false;
                    }

                    var playerIds = apiResponse.Data.Select(p => p.Id).ToHashSet();
                    var localPlayers = _playerRepository.GetDb().Where(p => playerIds.Contains(p.Id)).ToList();

                    var newPlayers = new List<PlayerData>();

                    bool isModified = false;
                    foreach (var player in apiResponse.Data)
                    {
                        var existingPlayer = localPlayers.SingleOrDefault(p => p.Id == player.Id);

                        if (existingPlayer != null)
                        {
                            if (existingPlayer.JerseyNumber != player.JerseyNumber)
                            {
                                existingPlayer.JerseyNumber = player.JerseyNumber;
                                isModified = true;
                            }
                        }
                        else
                        {
                            newPlayers.Add(player); // Yeni oyuncuları toplu olarak eklemek için listeye ekle
                            isModified = true;
                        }
                    }

                    if (newPlayers.Any())
                    {
                        await _playerRepository.AddRangeAsync(newPlayers); // Tüm yeni oyuncuları toplu olarak ekle
                    }

                    if (isModified)
                    {
                        await _playerRepository.SaveChangesAsync(); // Değişiklikleri tek seferde kaydet
                    }

                    await LogToDatabase("Info", operationName, "Fetched data from API successfully");

                    PrintAllPlayers();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during {Operation}", operationName);
                await LogToDatabase("Error", operationName, "An error occurred during processing", ex.Message);
                return false;
            }
        }

        private void PrintLeagueData(League league)
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

        private void PrintAllLeagues()
        {
            var leagues = _leagueRepository.GetAllAsync().Result.ToList();
            foreach (var league in leagues)
            {
                PrintLeagueData(league);
            }
        }

        private void PrintPlayerData(PlayerData player)
        {
            Console.WriteLine($"Id: {player.Id},\n" +
                              $"TransferId: {CheckNull(player.TransferId)},\n" +
                              $"PlayerId: {CheckNull(player.PlayerId)},\n" +
                              $"TeamId: {CheckNull(player.TeamId)},\n" +
                              $"PositionId: {CheckNull(player.PositionId)},\n" +
                              $"DetailedPositionId: {CheckNull(player.DetailedPositionId)},\n" +
                              $"Start: {CheckNull(player.Start)},\n" +
                              $"End: {CheckNull(player.End)},\n" +
                              $"Captain: {CheckNull(player.Captain)},\n" +
                              $"JerseyNumber: {CheckNull(player.JerseyNumber)}\n");
        }

        private void PrintAllPlayers()
        {
            var players = _playerRepository.GetAllAsync().Result.ToList();
            foreach (var player in players)
            {
                PrintPlayerData(player);
            }
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

        private string CheckNull(object value)
        {
            return value == null ? "boş" : value.ToString();
        }
    }
}
