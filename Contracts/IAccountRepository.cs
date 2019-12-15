using Entities.Helpers;
using Entities.Models;
using System;

namespace Contracts
{
	public interface IAccountRepository:IRepositoryBase<Account>
    {
		PagedList<Account> GetAccountsByOwner(Guid ownerId, AccountParameters parameters);
		Account GetAccountByOwner(Guid ownerId, Guid id);
	}
}
