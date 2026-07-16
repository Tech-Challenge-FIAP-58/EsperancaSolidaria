using CampaignService.Domain.Models;
using CampaignService.Infra.Mongo.Collections;
using CampaignService.Infra.Repositories.Interfaces;

namespace CampaignService.Infra.Repositories
{
	public class CampaignRepository(MongoCollections collections) : MongoRepository<Campaign>(collections.Campaigns), ICampaignRepository
	{
		public async Task CreateCampaign(Campaign campaign)
		{
			await Add(campaign);
		}

		public async Task UpdateCampaign(Campaign campaign)
		{
			await Update(campaign);
		}

		public async Task<IEnumerable<Campaign>> GetAllCampaigns()
		{
			return await GetAll();
		}

		public async Task<Campaign?> GetCampaignById(Guid id)
		{
			return await GetById(id);
		}
	}
}
