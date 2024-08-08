using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using FutbolApi.Services;
using FutbolApi.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Components.Forms;

namespace FutbolApi
{
    class Program
    {
        private static readonly string token = "XUibYHFwyAWxyfCi0lxHA6F96o5CMGHuWPUsljoHcG3flCa9dOOoYTdci48n";
        private static ApiService apiService;

        static async Task Main(string[] args)
        {
            var logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();
            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        await context.Database.MigrateAsync();

                        var fetchService = new FetchService(context, services.GetRequiredService<ILogger<FetchService>>());
                        apiService = new ApiService(token, fetchService, services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ApiService>>());

                        await ShowMenu();
                    }
                    catch (DbUpdateException ex)
                    {
                        logger.Error(ex, "An error occurred while creating the database.");
                        Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred while creating the database.");
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred during application initialization.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            hostContext.Configuration.GetConnectionString("DefaultConnection")));
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog();
                    });
                });

        private static async Task ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("Menü:");
                Console.WriteLine("1. Takımların getirilmesi");
                Console.WriteLine("2. liglerinin getirilmesi");
                Console.WriteLine("3. Istenen ligin getirilmesi");
                Console.WriteLine("Çıkmak için q'ya basın");

                string choice = Console.ReadLine();
                string exit = "Çıkıldı";

                await Console.Out.WriteLineAsync(nameof(exit));//Sensei öğretisi


                if (string.Equals( choice , nameof(ConsoleKey.Q),StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                switch (choice)
                {
                    case "1":
                        await apiService.GetSportAsync();
                        break;
                    case "2":
                        await apiService.GetLeaguesAsync();
                        break;
                    case "3":
                        Console.Write("Ligin ID'sini girin: ");
                        string leagueId = Console.ReadLine();
                        await apiService.GetLeagueDetailsAsync(leagueId);
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                        break;
                }
            }
        }
    }
}
