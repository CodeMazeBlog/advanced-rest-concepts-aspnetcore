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
		private ISortHelper<Account> _sortHelper;
		private IDataShaper<Account> _dataShaper;

		public AccountRepository(RepositoryContext repositoryContext, ISortHelper<Account> sortHelper, IDataShaper<Account> dataShaper)
			: base(repositoryContext)
		{
			_sortHelper = sortHelper;
			_dataShaper = dataShaper;
		}

		public PagedList<ShapedEntity> GetAccountsByOwner(Guid ownerId, AccountParameters parameters)
		{
			var accounts = FindByCondition(a => a.OwnerId.Equals(ownerId));

			var sortedAccounts = _sortHelper.ApplySort(accounts, parameters.OrderBy);

			var shapedAccounts = _dataShaper.ShapeData(sortedAccounts, parameters.Fields);

			return PagedList<ShapedEntity>.ToPagedList(shapedAccounts,
				parameters.PageNumber,
				parameters.PageSize);
		}

		public ShapedEntity GetAccountByOwner(Guid ownerId, Guid id, string fields)
		{
			var account = FindByCondition(a => a.OwnerId.Equals(ownerId) && a.Id.Equals(id)).SingleOrDefault();
			return _dataShaper.ShapeData(account, fields);
		}

		public Account GetAccountByOwner(Guid ownerId, Guid id)
		{
			return FindByCondition(a => a.OwnerId.Equals(ownerId) && a.Id.Equals(id)).SingleOrDefault();
		}
	}
}