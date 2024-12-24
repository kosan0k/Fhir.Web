using Fhir.Patients.Domain.Models;
using MediatR;

namespace Fhir.Patients.Web.Messages.Update
{
    public class UpdateResourceRequest<TResource>(TResource resource) : IRequest<UpdateResourceResponse<TResource>>
        where TResource : IResource
    {
        public TResource Resource { get; } = resource;
    }
}
