using CampaignService.Domain.Models;
using CampaignService.Infra.Mongo.Collections;
using CampaignService.Infra.Repositories.Interfaces;

namespace CampaignService.Infra.Repositories
{
    public class CampaignRepository(MongoCollections collections) : MongoRepository<Campaign>(collections.Campaigns), ICampaignRepository
    {

    }
}
