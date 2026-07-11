using MongoDB.Driver;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Models;
using UserService.Infra.Mongo;
using UserService.Infra.Mongo.Collections;

namespace UserService.Infra.Repository
{
	public class UserStatisticsRepository(MongoCollections collections) : IUserStatisticsRepository
	{
		private readonly IMongoCollection<UserDonationStat> _collection = collections.UserStatistics;

		/// <summary>
		/// Insere o registro da doação. Diferente do MongoRepository.Register — que
		/// traduz duplicata em exceção — aqui a colisão de _id (mesmo DonationId de uma
		/// reentrega) é engolida e devolvida como false, tornando o consumo idempotente.
		/// </summary>
		public async Task<bool> Add(UserDonationStat stat)
		{
			try
			{
				await _collection.InsertOneAsync(stat);
				return true;
			}
			catch (MongoWriteException ex) when (ex.IsDuplicateKey())
			{
				return false;
			}
		}

		public async Task<long> CountByUser(Guid userId) =>
			await _collection.CountDocumentsAsync(x => x.UserId == userId);

		public async Task<decimal> SumAmountByUser(Guid userId)
		{
			var docs = await _collection
				.Find(x => x.UserId == userId)
				.Project(x => x.Amount)
				.ToListAsync();

			return docs.Sum();
		}
	}
}
