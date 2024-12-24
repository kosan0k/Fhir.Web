using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Fhir.Patients.Database.Mongo
{
    public class MongoRepository<TResource> : IRepository<TResource>
        where TResource : IResource
    {
        private readonly IMongoCollection<TResource> _collection;

        public MongoRepository(IMongoCollection<TResource> collection)
        {
            _collection = collection;
        }

        public async Task<Result<IEnumerable<TResource>, Exception>> FindAsync(
            Expression<Func<TResource, bool>> expression,
            CancellationToken token)
        {
            Result<IEnumerable<TResource>, Exception> result;

            try
            {
                var cursor = await _collection.FindAsync(
                    filter: expression,
                    cancellationToken: token);

                result = Result.Success<IEnumerable<TResource>, Exception>(await cursor.ToListAsync(token));
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }

        public async Task<Result<TResource, Exception>> CreateAsync(
            TResource resource,
            CancellationToken token)
        {
            Result<TResource, Exception> result;

            try
            {
                await _collection.InsertOneAsync(
                    document: resource,
                    options: null,
                    cancellationToken: token);

                result = resource;
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }

        public async Task<UnitResult<Exception>> DeleteAsync(
            string id,
            CancellationToken token)
        {
            UnitResult<Exception> result;

            try
            {
                var deleteResult = await _collection.DeleteOneAsync(
                    filter: r => r.Id == id,
                    cancellationToken: token);

                result = deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0
                    ? UnitResult.Success<Exception>()
                    : UnitResult.Failure(new Exception($"Can not delete resource with id[{id}]"));
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }

        public async Task<UnitResult<Exception>> UpdateAsync(
            TResource resource,
            CancellationToken token)
        {
            UnitResult<Exception> result;

            try
            {
                var filter = Builders<TResource>.Filter.Eq(doc => doc.Id, resource.Id);

                var replaceResult = await _collection.ReplaceOneAsync(
                    filter: filter,
                    replacement: resource,
                    options: new ReplaceOptions(),
                    cancellationToken: token);

                result = replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount > 0
                    ? UnitResult.Success<Exception>()
                    : UnitResult.Failure(new Exception($"Could not update resource with id[{resource.Id}]"));
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }
    }
}
