using CampaignService.Domain.Entities;
using CampaignService.Domain.Entities.DTOs;
using CampaignService.Domain.Interfaces;
using CampaignService.Domain.Interfaces.MassTransit.Producer;
using CampaignService.Domain.Models;
using CampaignService.Infra.Repositories.Interfaces;
using EsperancaSolidaria.Contracts.Entities.Web;
using EsperancaSolidaria.Contracts.Events;
using EsperancaSolidaria.Contracts.Services;

namespace CampaignService.Application.Services
{
    public class CampaignManagementService(
		ICampaignRepository repository, ICampaignLogRepository logRepository, 
		IDonationRejectedEventProducer donationRejectedEventProducer) : BaseService, ICampaignManagementService
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
			await logRepository.WriteLog(campaign.Id, $"Campanha cancelada em {DateTime.Now}.");

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
			await logRepository.WriteLog(campaign.Id, $"Campanha criada em {DateTime.Now}.");

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
			await logRepository.WriteLog(campaign.Id, $"Campanha atualizada em {DateTime.Now}.");

			return Ok(true, message: $"Campanha #{campaign.Id} atualizada com sucesso.");
		}

		public async Task AddDonation(AddDonationDto dto)
		{
			var campaign = await repository.GetCampaignById(dto.CampaignId);
			if (campaign == null)
			{
				await donationRejectedEventProducer.Publish(new DonationRejectedEvent
				{
					CampaignId = dto.CampaignId,
					DonationId = dto.DonationId,
					Message = "CampaignService: Campanha não encontrada"
				});
				return;
			}

			try
			{
				var campaignDonationUpdated = new CampaignDonationUpdated(campaign);
				campaignDonationUpdated.AddDonation(dto.Amount);
			}
			catch (Exception ex)
			{
				await donationRejectedEventProducer.Publish(new DonationRejectedEvent
				{
					CampaignId = dto.CampaignId,
					DonationId = dto.DonationId,
					Message = $"CampaignService (BadRequest): {ex.Message}"
				});
				return;
			}

			await repository.UpdateCampaign(campaign);
			await logRepository.WriteLog(campaign.Id, $"Doação de {dto.Amount:C} adicionada à campanha em {DateTime.Now}.");
		}
    }
}
