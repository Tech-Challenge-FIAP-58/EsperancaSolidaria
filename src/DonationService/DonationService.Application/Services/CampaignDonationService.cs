using DonationService.Domain.Entities;
using DonationService.Domain.Entities.DTOs;
using DonationService.Domain.Interfaces;
using DonationService.Domain.Interfaces.MassTransit.Producer;
using DonationService.Domain.Models;
using DonationService.Infra.Repositories.Interfaces;
using EsperancaSolidaria.Contracts.Entities.Web;
using EsperancaSolidaria.Contracts.Events;
using EsperancaSolidaria.Contracts.Services;

namespace DonationService.Application.Services
{
	public class CampaignDonationService(
		IDonationReceivedEventProducer donationReceivedEventProducer, IDonationRepository donationRepository, 
		IDonationLogRepository donationLogRepository) : BaseService, ICampaignDonationServiceDonationService
	{
		public async Task<IApiResponse<bool>> CreateDonation(CreateDonationDto dto)
		{
			Donation donation;

			try
			{
				var donationReceived = new DonationReceived();
				donation = donationReceived.CreateDonation(dto.DonorUserId, dto.CampaignId, dto.Amount);
			}
			catch (Exception ex)
			{
				return BadRequest<bool>(ex.Message);
			}

			await donationReceivedEventProducer.Publish(new DonationReceivedEvent
			{
				DonorUserId = donation.DonorUserId,
				CampaignId = donation.CampaignId,
				Amount = donation.Amount
			});

			await donationRepository.CreateDonation(donation);

			await donationLogRepository.
				WriteLog(donation.Id, dto.CampaignId, $"Doação publicada: DonorUserId={dto.DonorUserId}, Amount={dto.Amount}");

			return Created(true, "Doação criada com sucesso.");
		}

		public async Task<IApiResponse<List<Donation>>> GetDonationsByCampaignId(Guid campaignId)
		{
			var donations = await donationRepository.GetDonations(campaignId);
			if (donations == null || !donations.Any())
			{
				return NotFound<List<Donation>>($"Nenhuma doação encontrada para a campanha {campaignId}");
			}

			return Ok(donations.ToList());
		}

		public async Task<IApiResponse<bool>> RejectDonation(DonationRejectedDto dto)
		{
			await donationRepository.DeactivateDonation(dto.DonationId);
			await donationLogRepository.WriteLog(dto.DonationId, dto.CampaignId, $"Doação cancelada/rejeitada: {dto.Message}");

			return Ok(true);
		}
	}
}
