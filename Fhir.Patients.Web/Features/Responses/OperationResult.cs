using CSharpFunctionalExtensions;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Fhir.Patients.Web.Features.Responses
{
    public class OperationResult : IResult
    {
        private readonly UnitResult<Exception> _result;

        public OperationResult(UnitResult<Exception> result)
        {
            _result = result;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            IResult result;

            if (_result.IsSuccess)
            {
                result = TypedResults.Ok();
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
