using CampaignService.Domain.Models;
using CampaignService.Infra.Mongo.Collections;
using CampaignService.Infra.Repositories.Interfaces;

namespace CampaignService.Infra.Repositories
{
	public class CampaignLogRepository(MongoCollections collections) : MongoRepository<CampaignLog>(collections.CampaignLogs), ICampaignLogRepository
	{
		public async Task WriteLog(Guid campaignId, string message)
		{
			await Add(new CampaignLog(campaignId, message));
		}
	}
}
