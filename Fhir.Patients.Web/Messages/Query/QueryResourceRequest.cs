using Fhir.Patients.Domain.Models;
using MediatR;

namespace Fhir.Patients.Web.Messages.Query
{
    public class QueryResourceRequest<TResource> : IRequest<QueryResourceResponse<TResource>> 
        where TResource : IResource
    {
    }
}
