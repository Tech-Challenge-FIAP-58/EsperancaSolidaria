using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Models;
using EsperancaSolidaria.Contracts.Entities.Web;

namespace CampaignService.Domain.Interfaces
{
    public interface ICampaignManagementService
    {
        Task<IApiResponse<List<Campaign>>> GetAll();
        Task<IApiResponse<Campaign?>> GetById(Guid id);
        Task<IApiResponse<bool>> Create(CreateCampaignDto dto);
        Task<IApiResponse<bool>> Update(UpdateCampaignDto dto);
        Task<IApiResponse<bool>> CancelCampaign(Guid id);
        Task AddDonation(AddDonationDto dto);
	}
}
