using Fhir.Patients.Web.Features.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Fhir.Patients.Web.Models;

[ModelBinder(typeof(SearchParametersModelBinder))]
public class SearchParameters
{
    public required ILookup<string, string> Parameters { get; init; }
}
