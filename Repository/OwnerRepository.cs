using Contracts;
using Entities;
using Entities.Extensions;
using Entities.Helpers;
using Entities.Models;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repository
{
	public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
	{
		private ISortHelper<Owner> _sortHelper;

		public OwnerRepository(RepositoryContext repositoryContext, ISortHelper<Owner> sortHelper)
			: base(repositoryContext)
		{
			_sortHelper = sortHelper;
		}

		public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
		{
			var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
										o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

			SearchByName(ref owners, ownerParameters.Name);

			_sortHelper.ApplySort(owners, ownerParameters.OrderBy);

			return PagedList<Owner>.ToPagedList(owners,
				ownerParameters.PageNumber,
				ownerParameters.PageSize);
		}

		private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
		{
			if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
				return;

			if (string.IsNullOrEmpty(ownerName))
				return;

			owners = owners.Where(o => o.Name.ToLowerInvariant().Contains(ownerName.Trim().ToLowerInvariant()));
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