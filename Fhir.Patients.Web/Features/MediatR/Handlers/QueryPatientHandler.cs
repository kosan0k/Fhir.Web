using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Features.Search;
using Fhir.Patients.Web.Messages.Query;
using MediatR;

namespace Fhir.Patients.Web.Features.MediatR.Handlers
{
    public class QueryPatientHandler : IRequestHandler<QueryResourceRequest<Patient>, QueryResourceResponse<Patient>>
    {
        private readonly IRepository<Patient> _patientsRepository;

        public QueryPatientHandler(IRepository<Patient> patientsRepository)
        {
            _patientsRepository = patientsRepository;
        }

        public async Task<QueryResourceResponse<Patient>> Handle(
            QueryResourceRequest<Patient> request,
            CancellationToken cancellationToken)
        {
            var buildExpressionResult = FhirSearchExpressions.BuildExpression<Patient>(request.SearchParameters);

            if (buildExpressionResult.IsFailure)
                return new QueryResourceResponse<Patient>(buildExpressionResult.Error);

            var result = await _patientsRepository.FindAsync(buildExpressionResult.Value, cancellationToken);

            return new QueryResourceResponse<Patient>(result);
        }
    }
}
