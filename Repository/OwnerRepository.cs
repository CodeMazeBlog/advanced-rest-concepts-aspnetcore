using Contracts;
using Entities;
using Entities.Extensions;
using Entities.Helpers;
using Entities.Models;
using System;
using System.Linq;

namespace Repository
{
	public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
	{
		public OwnerRepository(RepositoryContext repositoryContext)
			: base(repositoryContext)
		{
		}

		public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
		{
			var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
										o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

			SearchByName(ref owners, ownerParameters.Name);

			return PagedList<Owner>.ToPagedList(owners.ToList().OrderBy(on => on.Name),
				ownerParameters.PageNumber,
				ownerParameters.PageSize);
		}

		private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
		{
			if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
				return;

			owners = owners.Where(o => o.Name.ToLower().Contains(ownerName.Trim().ToLower()));
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