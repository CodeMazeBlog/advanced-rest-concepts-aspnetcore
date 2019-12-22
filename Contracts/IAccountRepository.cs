using Entities.Helpers;
using Entities.Models;
using System;

namespace Contracts
{
	public interface IAccountRepository : IRepositoryBase<Account>
	{
		PagedList<Entity> GetAccountsByOwner(Guid ownerId, AccountParameters parameters);
		Entity GetAccountByOwner(Guid ownerId, Guid id, string fields);
		Account GetAccountByOwner(Guid ownerId, Guid id);
	}
}
