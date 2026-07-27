using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using CampaignService.Domain.Models;
using EsperancaSolidaria.Contracts.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignService.WebApi.Controllers
{
	[Authorize]
	public class CampaignController(ILogger<CampaignController> logger, ICampaignManagementService service) : StandardController
	{
		[HttpGet]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetPublicCampaigns()
		{
			logger.LogInformation("Get all public campaigns");
			return await ExecuteAsync(() => service.GetPublicCampaigns());
		}

		[HttpGet]
		[Authorize(Roles = Roles.GestorONG)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetCampaigns()
		{
			logger.LogInformation("Get all campaigns");
			return await ExecuteAsync(() => service.GetAll());
		}

		[HttpGet("{id}")]
		[Authorize(Roles = Roles.GestorONG)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetCampaign(Guid id)
		{
			logger.LogInformation("Get campaign by id: {id}", id);
			return await ExecuteAsync(() => service.GetById(id));
		}

		[HttpPost]
		[Authorize(Roles = Roles.GestorONG)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> CreateCampaign(CreateCampaignDto dto)
		{
			logger.LogInformation("Create campaign with title: {title}", dto.Title);
			return await ExecuteAsync(() => service.Create(dto));
		}

		[HttpPut]
		[Authorize(Roles = Roles.GestorONG)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> UpdateCampaign(UpdateCampaignDto dto)
		{
			logger.LogInformation("Update campaign with id: {id}", dto.Id);
			return await ExecuteAsync(() => service.Update(dto));
		}

		[HttpPut("{id}/cancel")]
		[Authorize(Roles = Roles.GestorONG)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> CancelCampaign(Guid id)
		{
			logger.LogInformation("Cancel campaign with id: {id}", id);
			return await ExecuteAsync(() => service.CancelCampaign(id));
		}
	}
}
