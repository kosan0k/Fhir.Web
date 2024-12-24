using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Models;

namespace Fhir.Patients.Web.Messages.Query
{
    public class QueryResourceResponse<TResource>(Result<IEnumerable<TResource>, Exception> result) 
        where TResource : IResource
    {
        public Result<IEnumerable<TResource>, Exception> Value { get; } = result;
    }
}
