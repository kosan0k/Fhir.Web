using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Models;

namespace Fhir.Patients.Web.Messages.Update
{
    public class UpdateResourceResponse<TResource>(UnitResult<Exception> result)
        where TResource : IResource
    {
        public UnitResult<Exception> Result { get; } = result;
    }
}
