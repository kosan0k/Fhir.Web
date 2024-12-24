using Fhir.Patients.Domain.Models;
using MediatR;

namespace Fhir.Patients.Web.Messages.Delete
{
    public class DeleteResourceRequest<TResource>(string id) : IRequest<DeleteResourceResponse<TResource>>
        where TResource : IResource
    {
        public string Id { get; } = id;
    }
}
