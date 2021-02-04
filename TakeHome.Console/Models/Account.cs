using System;
using System.Collections.Generic;

namespace TakeHome.Console.Models
{
	public record Account
	{
		public string Id { get; init; }
		public string Name { get; init; }
		public string Email { get; init; }
	}

	class IdComparer : IEqualityComparer<Account>
	{
		public bool Equals(Account x, Account y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Account account)
		{
			if (account is null) return 0;

			return account.Id.GetHashCode();
		}
	}
}

