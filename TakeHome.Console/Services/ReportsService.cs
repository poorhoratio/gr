using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Console.Models;
using TakeHome.Console.Queries;

namespace TakeHome.Console.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IGetAccounts _getOldAccounts;
        private readonly IGetAccounts _getNewAccounts;
        private readonly ILogger<IReportsService> _logger;
        private readonly string _outputPath;

        public ReportsService(IGetAccounts getOldAccounts, IGetAccounts getNewAccounts, string outputPath, ILogger<IReportsService> logger)
        {
            _getOldAccounts = getOldAccounts;
            _getNewAccounts = getNewAccounts;
            _outputPath = outputPath;
            _logger = logger;
        }

        public List<AccountChange> GetChanges()
        {
            _logger.LogInformation("Getting changes");
            var oldAccounts = _getOldAccounts.GetAll().ToList();
            _logger.LogInformation($"Old account count: {oldAccounts.Count}");

            var newAccounts = _getNewAccounts.GetAll().ToList();
            _logger.LogInformation($"New account count: {oldAccounts.Count}");

            var oldRecordsMissingInNewList = oldAccounts
                .Except(newAccounts, new IdComparer())
                .Select(oa => new AccountChange
                {
                    ChangeType = ChangeType.Missing,
                    Id = oa.Id,
                    OldName = oa.Name,
                    OldEmail = oa.Email
                })
                .ToList();

            var recordsAddedSinceMigration = newAccounts
                .Except(oldAccounts, new IdComparer())
                .Select(na => new AccountChange
                {
                    ChangeType = ChangeType.New,
                    Id = na.Id,
                    NewName = na.Name,
                    NewEmail = na.Email
                })
                .ToList();

            var oldDictionary = oldAccounts.ToDictionary(a => a.Id, a => a);
            var corruptedAccounts = newAccounts
                .Where(na => oldDictionary.ContainsKey(na.Id) && oldDictionary[na.Id] != na)
                .Select(na =>
                {
                    var oldAccount = oldDictionary[na.Id];
                    return new AccountChange
                    {
                        ChangeType = ChangeType.Corrupted,
                        Id = na.Id,
                        OldName = oldAccount.Name,
                        OldEmail = oldAccount.Email,
                        NewName = na.Name,
                        NewEmail = na.Email
                    };
                })
                .ToList();

            var allChanges = oldRecordsMissingInNewList
                .Concat(recordsAddedSinceMigration)
                .Concat(corruptedAccounts)
                .ToList();

            _logger.LogInformation($"Old records missing in new list: {oldRecordsMissingInNewList.Count}");
            _logger.LogInformation($"Records added since migration:   {recordsAddedSinceMigration.Count}");
            _logger.LogInformation($"Corrupted accounts found:        {corruptedAccounts.Count}");
            _logger.LogInformation($"Total changes found:             {allChanges.Count}");

            return allChanges.ToList();
        }

        public void GenerateMigrationReport()
        {
            var headerRow = "Change,Id,OldName,OldEmail,NewName,NewEmail\n";
            var changes = GetChanges();
            var filename = Path.Combine(_outputPath, "ChangeReport.csv");
            _logger.LogInformation($"Creating report: {filename}");
            File.WriteAllText(filename, headerRow);
            File.AppendAllLines(filename, changes.Select(c => c.Csv));
        }
    }
}
