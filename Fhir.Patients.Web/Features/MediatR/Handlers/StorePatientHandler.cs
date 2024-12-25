using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Store;
using MediatR;

namespace Fhir.Patients.Web.Features.MediatR.Handlers
{
    public class StorePatientHandler : IRequestHandler<StoreResourceRequest<Patient>, StoreResourceResponse<Patient>>
    {
        private readonly IRepository<Patient> _patientsRepository;

        public StorePatientHandler(IRepository<Patient> patientsRepository)
        {
            _patientsRepository = patientsRepository;
        }

        public async Task<StoreResourceResponse<Patient>> Handle(StoreResourceRequest<Patient> request, CancellationToken cancellationToken)
        {
            var result = await _patientsRepository.CreateAsync(request.Resource, cancellationToken);
            return new StoreResourceResponse<Patient>(result);
        }
    }
}
