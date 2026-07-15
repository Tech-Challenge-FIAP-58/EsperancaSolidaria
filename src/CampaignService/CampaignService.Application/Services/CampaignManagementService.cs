using CampaignService.Domain.Entities;
using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using CampaignService.Domain.Models;
using CampaignService.Infra.Repositories.Interfaces;
using EsperancaSolidaria.Contracts.Entities.Web;
using EsperancaSolidaria.Contracts.Services;

namespace CampaignService.Application.Services
{
    public class CampaignManagementService(ICampaignRepository repository) : BaseService, ICampaignManagementService
    {
        public  async Task<IApiResponse<bool>> CancelCampaign(Guid id)
        {
			var campaign = await repository.GetCampaignById(id);
			if (campaign == null)
			{
				return NotFound<bool>("Campanha não encontrada.");
			}

			try
			{
				var campaignUpdated = new CampaignUpdated(campaign);
				campaignUpdated.CancelCampaign();
			}
			catch (Exception ex)
			{
				return BadRequest<bool>(ex.Message);
			}

			await repository.UpdateCampaign(campaign);

            return Ok(true, message: $"Campanha #{campaign.Id} cancelada com sucesso.");
		}

        public async Task<IApiResponse<bool>> Create(CreateCampaignDto dto)
        {
			Campaign campaign;

            try
            {
				var campaignCreated = new CampaignCreated();
				campaign = campaignCreated.CreateCampaign(
					dto.Title, dto.Description, 
					dto.StartDate, dto.EndDate, dto.FinancialTarget);
			}
            catch (Exception ex)
			{
				return BadRequest<bool>(ex.Message);
			}
            
            await repository.CreateCampaign(campaign);

            return Created(true, message: $"Campanha #{campaign.Id} criada com sucesso.");
        }

		public async Task<IApiResponse<List<Campaign>>> GetAll()
		{
            var campaigns = await repository.GetAllCampaigns();
            return Ok(campaigns.ToList());
		}

		public async Task<IApiResponse<Campaign?>> GetById(Guid id)
		{
			var campaign = await repository.GetCampaignById(id);
            if (campaign == null)
            {
                return NotFound<Campaign?>("Campanha não encontrada.");
            }

            return Ok<Campaign?>(campaign);
		}

		public async Task<IApiResponse<bool>> Update(UpdateCampaignDto dto)
        {
			var campaign = await repository.GetCampaignById(dto.Id);
			if (campaign == null)
			{
				return NotFound<bool>("Campanha não encontrada.");
			}

			try
			{
				var campaignUpdated = new CampaignUpdated(campaign);
				campaignUpdated.UpdateCampaign(
					dto.Title, dto.Description,
					dto.StartDate, dto.EndDate, dto.FinancialTarget);
			}
			catch (Exception ex)
			{
				return BadRequest<bool>(ex.Message);
			}

			await repository.UpdateCampaign(campaign);

			return Ok(true, message: $"Campanha #{campaign.Id} atualizada com sucesso.");
		}

		public async Task<IApiResponse<bool>> AddDonation(AddDonationDto dto)
		{
			var campaign = await repository.GetCampaignById(dto.Id);
			if (campaign == null)
			{
				return NotFound<bool>("Campanha não encontrada.");
			}

			try
			{
				var campaignDonationUpdated = new CampaignDonationUpdated(campaign);
				campaignDonationUpdated.AddDonation(dto.Amount);
			}
			catch (Exception ex)
			{
				return BadRequest<bool>(ex.Message);
			}

			await repository.UpdateCampaign(campaign);

			return Ok(true, message: $"Doação de {dto.Amount:C} adicionada à campanha #{campaign.Id} com sucesso.");
		}
    }
}
