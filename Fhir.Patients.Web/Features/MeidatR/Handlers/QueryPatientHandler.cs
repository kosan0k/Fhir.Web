﻿using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Query;
using MediatR;

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
            var expression = request.SearchParameters is null
                ? _ => true
                : request.SearchParameters.BuildExpression();

            var result = await _patientsRepository.FindAsync(expression, cancellationToken);

            return new QueryResourceResponse<Patient>(result);
        }
    }
}
