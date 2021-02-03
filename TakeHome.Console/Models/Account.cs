﻿using System;
using System.Collections.Generic;

namespace TakeHome.Console.Models
{
	public record Account
	{
		public string Id { get; }
		public string Name { get; }
		public string Email { get; }
	}

	class IdComparer : IEqualityComparer<Account>
	{
		public bool Equals(Account x, Account y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Account account)
		{
			if (Object.ReferenceEquals(account, null)) return 0;

			return account.Id.GetHashCode();
		}
	}
}

