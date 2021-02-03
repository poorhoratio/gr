using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using TakeHome.Console.Queries;
using TakeHome.Console.Services;

namespace GuaranteedRate.TakeHome
{
    class Program
    {
        private static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"Migration Report v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            Console.WriteLine("-------------------------------------");

            var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            var oldConnectionString = _configuration.GetConnectionString("OldConnectionString");
            var newConnectionString = _configuration.GetConnectionString("NewConnectionString");

            var oldAccountsQuery = new GetAccounts(new Npgsql.NpgsqlConnection(oldConnectionString));
            var newAccountsQuery = new GetAccounts(new Npgsql.NpgsqlConnection(newConnectionString));

            var reportsService = new ReportsService(oldAccountsQuery, newAccountsQuery);
            reportsService.GenerateReports();
        }
    }
}
