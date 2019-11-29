using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using System;
using System.Linq;

namespace Repository
{
	public class AccountRepository : RepositoryBase<Account>, IAccountRepository
	{
		public AccountRepository(RepositoryContext repositoryContext)
			: base(repositoryContext)
		{

		}

		public PagedList<Account> GetAccountsByOwner(Guid ownerId, AccountParameters parameters)
		{
			var accounts = FindByCondition(a => a.OwnerId.Equals(ownerId));

			return PagedList<Account>.ToPagedList(accounts,
				parameters.PageNumber,
				parameters.PageSize);
		}

		public Account GetAccountByOwner(Guid ownerId, Guid id)
		{
			return FindByCondition(a => a.OwnerId.Equals(ownerId) && a.Id.Equals(id)).SingleOrDefault();
		}
	}
}
