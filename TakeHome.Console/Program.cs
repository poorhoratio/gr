using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TakeHome.Console.Queries;
using TakeHome.Console.Services;

namespace GuaranteedRate.TakeHome
{
    public class Options
    {
        [Option('p', "path", Required = false, HelpText = "Output path. If blank exe directory will be used.")]
        public string Path { get; set; }
    }

    class Program
    {
        private static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"Migration Report v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            Console.WriteLine("-------------------------------------");
            
            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));

            var logger = loggerFactory.CreateLogger<IReportsService>();


            var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            var oldConnectionString = _configuration.GetConnectionString("OldConnectionString");
            var newConnectionString = _configuration.GetConnectionString("NewConnectionString");

            var oldAccountsQuery = new GetAccounts(new Npgsql.NpgsqlConnection(oldConnectionString));
            var newAccountsQuery = new GetAccounts(new Npgsql.NpgsqlConnection(newConnectionString));

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var path = o.Path ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    if (!Directory.Exists(path))
                    {
                        logger.LogError($"The specified path ({path}) does not exist. The job could not run.");
                    }
                    else
                    {
                        try
                        {
                            var reportsService = new ReportsService(oldAccountsQuery, newAccountsQuery, path, logger);
                            reportsService.GenerateMigrationReport();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Uh oh, we ran into an error: {ex.Message}");
                        }
                    }
                });
        }
    }
}
