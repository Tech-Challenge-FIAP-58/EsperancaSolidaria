using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using EsperancaSolidaria.Contracts.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CampaignService.WebApi.Controllers
{
	public class CampaignController(ILogger<CampaignController> logger, ICampaignManagementService service) : StandardController
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetCampaigns()
		{
			logger.LogInformation("Get all campaigns");
			return await ExecuteAsync(() => service.GetAll());
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetCampaign(Guid id)
		{
			logger.LogInformation("Get campaign by id: {id}", id);
			return await ExecuteAsync(() => service.GetById(id));
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateCampaign(CreateCampaignDto dto)
		{
			logger.LogInformation("Create campaign with title: {title}", dto.Title);
			return await ExecuteAsync(() => service.Create(dto));
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateCampaign(UpdateCampaignDto dto)
		{
			logger.LogInformation("Update campaign with id: {id}", dto.Id);
			return await ExecuteAsync(() => service.Update(dto));
		}

		[HttpPut("{id}/cancel")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CancelCampaign(Guid id)
		{
			logger.LogInformation("Cancel campaign with id: {id}", id);
			return await ExecuteAsync(() => service.CancelCampaign(id));
		}

		[HttpPost("donation")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddDonation(AddDonationDto dto)
		{
			logger.LogInformation("Add donation to campaign with id: {id}", dto.Id);
			return await ExecuteAsync(() => service.AddDonation(dto));
		}
	}
}
