using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Models;
using System.Linq.Expressions;

namespace Fhir.Patients.Domain.Contracts
{
    public interface IRepository<TResource> where TResource : IResource
    {
        public Task<Result<IEnumerable<TResource>, Exception>> FindAsync(
            Expression<Func<TResource, bool>> expression,
            CancellationToken token);

        public Task<Result<TResource, Exception>> CreateAsync(
            TResource resource,
            CancellationToken token);

        public Task<UnitResult<Exception>> UpdateAsync(
            TResource resource,
            CancellationToken token);

        public Task<UnitResult<Exception>> DeleteAsync(
            string id, 
            CancellationToken token);
    }
}
