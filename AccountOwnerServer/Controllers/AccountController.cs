using AccountOwnerServer.Filters;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountOwnerServer.Controllers
{
	[Route("api/owners/{ownerId}/accounts")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private ILoggerManager _logger;
		private IRepositoryWrapper _repository;
		private readonly LinkGenerator _linkGenerator;

		public AccountController(ILoggerManager logger,
			IRepositoryWrapper repository,
			LinkGenerator linkGenerator)
		{
			_logger = logger;
			_repository = repository;
			_linkGenerator = linkGenerator;
		}

		[HttpGet]
		[ServiceFilter(typeof(ValidateMediaTypeAttribute))]
		public IActionResult GetAccountsForOwner(Guid ownerId, [FromQuery] AccountParameters parameters)
		{
			var accounts = _repository.Account.GetAccountsByOwner(ownerId, parameters);

			var metadata = new
			{
				accounts.TotalCount,
				accounts.PageSize,
				accounts.CurrentPage,
				accounts.TotalPages,
				accounts.HasNext,
				accounts.HasPrevious
			};

			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

			_logger.LogInfo($"Returned {accounts.TotalCount} accounts from database.");

			var shapedAccounts = accounts.Select(o => o.Entity).ToList();

			var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

			if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
			{
				return Ok(shapedAccounts);
			}

			for (var index = 0; index < accounts.Count(); index++)
			{
				var accountLinks = CreateLinksForAccount(ownerId, accounts[index].Id, parameters.Fields);
				shapedAccounts[index].Add("Links", accountLinks);
			}

			var accountsWrapper = new LinkCollectionWrapper<Entity>(shapedAccounts);

			return Ok(CreateLinksForAccounts(accountsWrapper));
		}

		[HttpGet("{id}")]
		[ServiceFilter(typeof(ValidateMediaTypeAttribute))]
		public IActionResult GetAccountForOwner(Guid ownerId, Guid id, [FromQuery] string fields)
		{
			var account = _repository.Account.GetAccountByOwner(ownerId, id, fields);

			if (account.Id == Guid.Empty)
			{
				_logger.LogError($"Account with id: {id}, hasn't been found in db.");
				return NotFound();
			}

			var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

			if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
			{
				_logger.LogInfo($"Returned a shaped account with id: {id}");
				return Ok(account.Entity);
			}

			account.Entity.Add("Links", CreateLinksForAccount(account.Id, id, fields));

			return Ok(account.Entity);
		}

		private List<Link> CreateLinksForAccount(Guid ownerId, Guid id, string fields = "")
		{
			var links = new List<Link>
			{
				new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAccountForOwner), values: new { ownerId, id, fields }),
				"self",
				"GET"),
			};

			return links;
		}

		private LinkCollectionWrapper<Entity> CreateLinksForAccounts(LinkCollectionWrapper<Entity> accountsWrapper)
		{
			accountsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAccountsForOwner), values: new { }),
					"self",
					"GET"));

			return accountsWrapper;
		}
	}
}
