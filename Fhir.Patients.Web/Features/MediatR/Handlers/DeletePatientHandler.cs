using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Messages.Delete;
using MediatR;

namespace Fhir.Patients.Web.Features.MediatR.Handlers
{
    public class DeletePatientHandler : IRequestHandler<DeleteResourceRequest<Patient>, DeleteResourceResponse<Patient>>
    {
        private readonly IRepository<Patient> _patientsRepository;

        public DeletePatientHandler(IRepository<Patient> patientsRepository)
        {
            _patientsRepository = patientsRepository;
        }

        public async Task<DeleteResourceResponse<Patient>> Handle(DeleteResourceRequest<Patient> request, CancellationToken cancellationToken)
        {
            var result = await _patientsRepository.DeleteAsync(request.Id, cancellationToken);
            return new DeleteResourceResponse<Patient>(result);
        }
    }
}
