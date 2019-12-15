﻿using System;
using System.Linq;
using Contracts;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountOwnerServer.Controllers
{
	[Route("api/owner")]
	[ApiController]
	public class OwnerController : ControllerBase
	{
		private ILoggerManager _logger;
		private IRepositoryWrapper _repository;

		public OwnerController(ILoggerManager logger, IRepositoryWrapper repository)
		{
			_logger = logger;
			_repository = repository;
		}

		[HttpGet]
		public IActionResult GetOwners([FromQuery] OwnerParameters ownerParameters)
		{
			if (!ownerParameters.ValidYearRange)
			{
				return BadRequest("Max year of birth cannot be less than min year of birth");
			}

			var owners = _repository.Owner.GetOwners(ownerParameters);

			var metadata = new
			{
				owners.TotalCount,
				owners.PageSize,
				owners.CurrentPage,
				owners.TotalPages,
				owners.HasNext,
				owners.HasPrevious
			};

			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

			_logger.LogInfo($"Returned {owners.TotalCount} owners from database.");

			return Ok(owners);
		}

		[HttpGet("{id}", Name = "OwnerById")]
		public IActionResult GetOwnerById(Guid id)
		{
			var owner = _repository.Owner.GetOwnerById(id);

			if (owner.IsEmptyObject())
			{
				_logger.LogError($"Owner with id: {id}, hasn't been found in db.");
				return NotFound();
			}
			else
			{
				_logger.LogInfo($"Returned owner with id: {id}");
				return Ok(owner);
			}
		}

		[HttpPost]
		public IActionResult CreateOwner([FromBody]Owner owner)
		{
			if (owner.IsObjectNull())
			{
				_logger.LogError("Owner object sent from client is null.");
				return BadRequest("Owner object is null");
			}

			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid owner object sent from client.");
				return BadRequest("Invalid model object");
			}

			_repository.Owner.CreateOwner(owner);
			_repository.Save();

			return CreatedAtRoute("OwnerById", new { id = owner.Id }, owner);
		}

		[HttpPut("{id}")]
		public IActionResult UpdateOwner(Guid id, [FromBody]Owner owner)
		{
			if (owner.IsObjectNull())
			{
				_logger.LogError("Owner object sent from client is null.");
				return BadRequest("Owner object is null");
			}

			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid owner object sent from client.");
				return BadRequest("Invalid model object");
			}

			var dbOwner = _repository.Owner.GetOwnerById(id);
			if (dbOwner.IsEmptyObject())
			{
				_logger.LogError($"Owner with id: {id}, hasn't been found in db.");
				return NotFound();
			}

			_repository.Owner.UpdateOwner(dbOwner, owner);
			_repository.Save();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteOwner(Guid id)
		{
			var owner = _repository.Owner.GetOwnerById(id);
			if (owner.IsEmptyObject())
			{
				_logger.LogError($"Owner with id: {id}, hasn't been found in db.");
				return NotFound();
			}

			_repository.Owner.DeleteOwner(owner);
			_repository.Save();

			return NoContent();
		}
	}
}