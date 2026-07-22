using DonationService.Domain.Entities.DTOs;
using DonationService.Domain.Models;
using EsperancaSolidaria.Contracts.Entities.Web;

namespace DonationService.Domain.Interfaces
{
	public interface ICampaignDonationServiceDonationService
	{
		Task<IApiResponse<bool>> CreateDonation(CreateDonationDto dto);
		Task<IApiResponse<List<Donation>>> GetDonationsByCampaignId(Guid campaignId);
	}
}
