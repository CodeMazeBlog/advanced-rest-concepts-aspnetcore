using Contracts;
using Entities;
using Entities.Extensions;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
	public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
	{
		public OwnerRepository(RepositoryContext repositoryContext)
			: base(repositoryContext)
		{
		}

		public IEnumerable<Owner> GetOwners()
		{
			return FindAll()
				.OrderBy(ow => ow.Name);
		}

		public Owner GetOwnerById(Guid ownerId)
		{
			return FindByCondition(owner => owner.Id.Equals(ownerId))
				.DefaultIfEmpty(new Owner())
				.FirstOrDefault();
		}

		public void CreateOwner(Owner owner)
		{
			Create(owner);
		}

		public void UpdateOwner(Owner dbOwner, Owner owner)
		{
			dbOwner.Map(owner);
			Update(dbOwner);
		}

		public void DeleteOwner(Owner owner)
		{
			Delete(owner);
		}
	}
}