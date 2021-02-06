using Dapper;
using Npgsql;
using System.Collections.Generic;
using TakeHome.Console.Models;

namespace TakeHome.Console.Queries
{
    public class GetAccounts : IGetAccounts
    {
        protected readonly NpgsqlConnection _connection;

        public GetAccounts(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Account> GetAll()
        {
            var accounts = _connection.Query<Account>("SELECT Id, Name, Email FROM Accounts");
            return accounts;
        }

        public Account GetById(string id)
        {
            var account = _connection.QueryFirstOrDefault<Account>("SELECT Id, Name, Email FROM Accounts WHERE Id = @Id", new { Id = id });
            return account;
        }
    }
}
