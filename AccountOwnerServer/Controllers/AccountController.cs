using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AccountOwnerServer.Controllers
{
	[Route("api/owner/{ownerId}/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private ILoggerManager _logger;
		private IRepositoryWrapper _repository;

		public AccountController(ILoggerManager logger,
			IRepositoryWrapper repository)
		{
			_logger = logger;
			_repository = repository;
		}

		[HttpGet]
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

			return Ok(accounts);
		}

		[HttpGet("{id}")]
		public IActionResult GetAccountForOwner(Guid ownerId, Guid id, [FromQuery] string fields)
		{
			var account = _repository.Account.GetAccountByOwner(ownerId, id, fields);

			if (account == default(Entity))
			{
				_logger.LogError($"Account with id: {id}, hasn't been found in db.");
				return NotFound();
			}

			return Ok(account);
		}
	}
}
