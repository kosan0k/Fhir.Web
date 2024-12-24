using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Query;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fhir.Patients.Web.Features.Responses
{
    internal class ResourceResult<TResource>(QueryResourceResponse<TResource> response) : IResult
        where TResource : IResource
    {
        private static readonly JsonSerializerOptions _options = new();

        private readonly QueryResourceResponse<TResource> _response = response;

        static ResourceResult()
        {
            _options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            IResult result;

            if (_response.Value.IsSuccess)
            {
                result = _response.Value.Value.Any()
                    ? TypedResults.Json(_response.Value.Value, _options)
                    : TypedResults.NoContent();
            }
            else
            {
                //TODO : decide which information about the error we can send as response
                result = _response.Value.Error is BadHttpRequestException
                    ? TypedResults.BadRequest(_response.Value.Error.Message)
                    : TypedResults.InternalServerError();
            }

            await result.ExecuteAsync(httpContext);
        }
    }
}
