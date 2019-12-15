using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		private RepositoryContext _repoContext;
		private IOwnerRepository _owner;
		private IAccountRepository _account;
		private ISortHelper<Owner> _ownerSortHelper;
		private ISortHelper<Account> _accountSortHelper;

		public IOwnerRepository Owner
		{
			get
			{
				if (_owner == null)
				{
					_owner = new OwnerRepository(_repoContext, _ownerSortHelper);
				}

				return _owner;
			}
		}

		public IAccountRepository Account
		{
			get
			{
				if (_account == null)
				{
					_account = new AccountRepository(_repoContext, _accountSortHelper);
				}

				return _account;
			}
		}

		public RepositoryWrapper(RepositoryContext repositoryContext,
			ISortHelper<Owner> ownerSortHelper,
			ISortHelper<Account> accountSortHelper)
		{
			_repoContext = repositoryContext;
			_ownerSortHelper = ownerSortHelper;
			_accountSortHelper = accountSortHelper;
		}

		public void Save()
		{
			_repoContext.SaveChanges();
		}
	}
}
