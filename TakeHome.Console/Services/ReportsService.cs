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
    public class ReportsService
    {
        private readonly IGetAccounts _getOldAccounts;
        private readonly IGetAccounts _getNewAccounts;

        public ReportsService(IGetAccounts getOldAccounts, IGetAccounts getNewAccounts)
        {
            _getOldAccounts = getOldAccounts;
            _getNewAccounts = getNewAccounts;
        }

        public List<AccountChange> GetChanges()
        {
            var oldAccounts = _getOldAccounts.GetAll();
            var newAccounts = _getNewAccounts.GetAll();

            var oldRecordsMissingInNewList = oldAccounts
                .Except(newAccounts, new IdComparer())
                .Select(oa => new AccountChange
                {
                    ChangeType = ChangeType.Missing,
                    Id = oa.Id,
                    OldName = oa.Name,
                    OldEmail = oa.Email
                });
            var recordsAddedSinceMigration = newAccounts
                .Except(oldAccounts, new IdComparer())
                .Select(na => new AccountChange
                {
                    ChangeType = ChangeType.New,
                    Id = na.Id,
                    NewName = na.Name,
                    NewEmail = na.Email
                });

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
                });

            var allChanges = oldRecordsMissingInNewList
                .Concat(recordsAddedSinceMigration)
                .Concat(corruptedAccounts);

            return allChanges.ToList();
        }

        public void GenerateReports()
        {
            var headerRow = "Change,Id,OldName,OldEmail,NewName,NewEmail\n";
            var changes = GetChanges();
            File.WriteAllText(@"e:\test\gr\changes.csv", headerRow);
            File.AppendAllLines(@"e:\test\gr\changes.csv", changes.Select(c => c.Csv));
            File.WriteAllLines(@"e:\test\gr\changes.sql", changes.Where(c => !string.IsNullOrWhiteSpace(c.Sql)).Select(c => c.Sql));
        }
    }
}
