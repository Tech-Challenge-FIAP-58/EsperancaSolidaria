using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using EsperancaSolidaria.Contracts.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CampaignService.WebApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CampaignController(ILogger<CampaignController> logger, ICampaignManagementService service) : StandardController
	{
		[HttpGet]
		public async Task<IActionResult> GetCampaigns()
		{
			logger.LogInformation("Get all campaigns");
			throw new NotImplementedException();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetCampaign(Guid id)
		{
			logger.LogInformation("Get campaign by id: {id}", id);
			throw new NotImplementedException();
		}

		[HttpPost]
		public async Task<IActionResult> CreateCampaign(CreateCampaignDto dto)
		{
			logger.LogInformation("Create campaign with title: {title}", dto.Title);
			return await TryMethodAsync(() => service.CreateCampaign(dto), logger);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateCampaign(UpdateCampaignDto dto)
		{
			logger.LogInformation("Update campaign with id: {id}", dto.Id);
			return await TryMethodAsync(() => service.UpdateCampaign(dto), logger);
		}

		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> CancelCampaign(Guid id)
		{
			logger.LogInformation("Cancel campaign with id: {id}", id);
			return await TryMethodAsync(() => service.CancelCampaign(id), logger);
		}
	}
}
