using System.Collections.Generic;
using TakeHome.Console.Models;

namespace TakeHome.Console.Queries
{
    public interface IGetAccounts
    {
        IEnumerable<Account> GetAll();
        Account GetById(string id);
    }
}
