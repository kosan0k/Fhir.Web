using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Features.Responses;
using Fhir.Patients.Web.Messages.Query;
using Fhir.Patients.Web.Messages.Store;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fhir.Patients.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController(ILogger<PatientsController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<PatientsController> _logger = logger;    

    private readonly IMediator _mediator = mediator;

    [HttpGet(Name = "GetPatients")]
    public async Task<IResult> Get()
    {
        QueryResourceResponse<Patient> response = await _mediator.Send(new QueryResourceRequest<Patient>());

        if(response.Value.IsFailure)
            _logger.LogError(response.Value.Error, "Error on retrieving patients");

        return new ResourceResult<Patient>(response);       
    }

    [HttpPost]
    public async Task<IResult> Post(Patient patient)
    {
        StoreResourceResponse<Patient> response = await _mediator.Send(new StoreResourceRequest<Patient>(patient));

        if(response.Result.IsFailure)
            _logger.LogError(response.Result.Error, "Error on store patient {patient}", patient);

        return new OperationResult(response.Result);
    }
}
