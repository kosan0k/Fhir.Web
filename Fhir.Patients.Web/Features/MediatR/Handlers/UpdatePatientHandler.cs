using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Update;
using MediatR;

namespace Fhir.Patients.Web.Features.MediatR.Handlers
{
    public class UpdatePatientHandler : IRequestHandler<UpdateResourceRequest<Patient>, UpdateResourceResponse<Patient>>
    {
        private readonly IRepository<Patient> _patientsRepository;

        public UpdatePatientHandler(IRepository<Patient> patientsRepository)
        {
            _patientsRepository = patientsRepository;
        }

        public async Task<UpdateResourceResponse<Patient>> Handle(UpdateResourceRequest<Patient> request, CancellationToken cancellationToken)
        {
            var result = await _patientsRepository.UpdateAsync(request.Resource, cancellationToken);
            return new UpdateResourceResponse<Patient>(result);
        }
    }
}
