using MongoDB.Driver;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;
using UserService.Infra.Mongo;
using UserService.Infra.Mongo.Collections;

namespace UserService.Infra.Repository
{
	public class UserStatisticsRepository(MongoCollections collections, IMongoClient client) : IUserStatisticsRepository
	{
		private readonly IMongoCollection<UserDonationStat> _stats = collections.UserStatistics;
		private readonly IMongoCollection<ProcessedMessage> _processed = collections.ProcessedMessages;
		private readonly IMongoClient _client = client;

		public async Task<bool> RegisterDonation(Guid messageId, Guid userId, decimal amount)
		{
			using var session = await _client.StartSessionAsync();
			session.StartTransaction();

			try
			{
				await _processed.InsertOneAsync(
					session,
					new ProcessedMessage { MessageId = messageId, ProcessedAt = DateTime.UtcNow });

				var filter = Builders<UserDonationStat>.Filter.Eq(x => x.UserId, userId);
				var update = Builders<UserDonationStat>.Update
					.Inc(x => x.TotalDonated, amount)
					.SetOnInsert(x => x.Guid, Guid.NewGuid())
					.SetOnInsert(x => x.CreatedAt, DateTimeOffset.Now)
					.Set(x => x.UpdatedAt, DateTimeOffset.Now);

				await _stats.UpdateOneAsync(
					session, filter, update, new UpdateOptions { IsUpsert = true });

				await session.CommitTransactionAsync();
				return true;
			}
			catch (MongoWriteException ex) when (ex.IsDuplicateKey())
			{
				await session.AbortTransactionAsync();
				return false;
			}
			catch
			{
				await session.AbortTransactionAsync();
				throw;
			}
		}

		public async Task<decimal> GetTotalByUser(Guid userId)
		{
			var doc = await _stats.Find(x => x.UserId == userId).FirstOrDefaultAsync();
			return doc?.TotalDonated ?? 0m;
		}
	}
}
