using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Query;

namespace Fhir.Patients.Web.Features.Responses
{
    internal class ResourceResult<TResource>(QueryResourceResponse<TResource> response) : IResult
        where TResource : IResource
    {
        private readonly QueryResourceResponse<TResource> _response = response;

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            IResult result;

            if (_response.Value.IsSuccess)
            {
                result = _response.Value.Value.Any()
                    ? TypedResults.Json(_response.Value.Value)
                    : TypedResults.NoContent();
            }
            else
            {
                //TODO : decide which information about the error we can send as response
                result = TypedResults.InternalServerError();
            }

            await result.ExecuteAsync(httpContext);
        }
    }
}
