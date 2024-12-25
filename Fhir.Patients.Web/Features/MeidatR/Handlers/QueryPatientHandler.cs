using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Query;
using MediatR;
using System.Linq.Expressions;

namespace Fhir.Patients.Web.Features.MeidatR.Handlers
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
            Expression<Func<Patient, bool>> expression;

            if (request.SearchParameters is null)
                expression = _ => true;
            else
            {
                var buildExpressionResult = request.SearchParameters.BuildExpression<Patient>();

                if (buildExpressionResult.IsSuccess)
                    expression = buildExpressionResult.Value;
                else
                    return new QueryResourceResponse<Patient>(buildExpressionResult.Error);
            }

            var result = await _patientsRepository.FindAsync(expression, cancellationToken);

            return new QueryResourceResponse<Patient>(result);
        }
    }
}
