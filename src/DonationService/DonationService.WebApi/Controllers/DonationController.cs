using DonationService.Domain.Entities.DTOs;
using DonationService.Domain.Interfaces;
using EsperancaSolidaria.Contracts.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DonationService.WebApi.Controllers
{
	public class DonationController(ILogger<DonationController> logger, ICampaignDonationServiceDonationService donationService) : StandardController
	{
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateDonation(CreateDonationDto dto)
		{
			logger.LogInformation("Create donation");
			return await ExecuteAsync(() => donationService.CreateDonation(dto));
		}

		[HttpGet("{campaignId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetCampaignDonations(Guid campaignId)
		{
			logger.LogInformation("Get donations by campaignId");
			return await ExecuteAsync(() => donationService.GetDonationsByCampaignId(campaignId));
		}
	}
}
