using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Features.Responses;
using Fhir.Patients.Web.Messages.Delete;
using Fhir.Patients.Web.Messages.Query;
using Fhir.Patients.Web.Messages.Store;
using Fhir.Patients.Web.Messages.Update;
using Fhir.Patients.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fhir.Patients.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController(ILogger<PatientsController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<PatientsController> _logger = logger;

    private readonly IMediator _mediator = mediator;

    [HttpGet(Name = "GetPatients")]
    [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IResult> Get([FromQuery] SearchParameters? searchParameters = null)
    {
        QueryResourceResponse<Patient> response = await _mediator.Send(new QueryResourceRequest<Patient>(searchParameters));

        if (response.Value.IsFailure)
            _logger.LogError(response.Value.Error, "Error on retrieving patients");

        return new ResourceResult<Patient>(response);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IResult> Post(PatientDto patient)
    {
        StoreResourceResponse<Patient> response = await _mediator.Send(new StoreResourceRequest<Patient>(patient.ToDomain()));

        if (response.Result.IsFailure)
            _logger.LogError(response.Result.Error, "Error on store patient {patient}", patient);

        return new OperationResult(response.Result);
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IResult> Delete(string id)
    {
        DeleteResourceResponse<Patient> response = await _mediator.Send(new DeleteResourceRequest<Patient>(id));

        if (response.Result.IsFailure)
            _logger.LogError(response.Result.Error, "Error on deleting patient with id {id}", id);

        return new OperationResult(response.Result);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IResult> Put(PatientDto patient)
    {
        UpdateResourceResponse<Patient> response = await _mediator.Send(new UpdateResourceRequest<Patient>(patient.ToDomain()));

        if (response.Result.IsFailure)
            _logger.LogError(response.Result.Error, "Error on update patient {patient}", patient);

        return new OperationResult(response.Result);
    }
}
