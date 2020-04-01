using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
	public class AccountRepository : RepositoryBase<Account>, IAccountRepository
	{
		private ISortHelper<Account> _sortHelper;

		public AccountRepository(RepositoryContext repositoryContext, ISortHelper<Account> sortHelper)
			: base(repositoryContext)
		{
			_sortHelper = sortHelper;
		}

		public PagedList<Account> GetAccountsByOwner(Guid ownerId, AccountParameters parameters)
		{
			var accounts = FindByCondition(a => a.OwnerId.Equals(ownerId));

			var sortedAccounts = _sortHelper.ApplySort(accounts, parameters.OrderBy);

			return PagedList<Account>.ToPagedList(sortedAccounts,
				parameters.PageNumber,
				parameters.PageSize);
		}

		public Account GetAccountByOwner(Guid ownerId, Guid id)
		{
			return FindByCondition(a => a.OwnerId.Equals(ownerId) && a.Id.Equals(id)).SingleOrDefault();
		}
	}
}