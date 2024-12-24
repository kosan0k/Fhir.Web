using Fhir.Patients.Domain.Models;
using MediatR;

namespace Fhir.Patients.Web.Messages.Store
{
    public class StoreResourceRequest<TResource>(TResource resource) : IRequest<StoreResourceResponse<TResource>>
        where TResource : IResource
    {
        public TResource Resource { get; } = resource;
    }
}
