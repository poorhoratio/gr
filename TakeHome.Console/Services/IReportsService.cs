using System.Collections.Generic;
using TakeHome.Console.Models;

namespace TakeHome.Console.Services
{
    public interface IReportsService
    {
        void GenerateMigrationReport();
        List<AccountChange> GetChanges();
    }
}