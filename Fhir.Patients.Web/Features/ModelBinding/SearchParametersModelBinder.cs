using Fhir.Patients.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fhir.Patients.Web.Features.ModelBinding
{
    public class SearchParametersModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryString = bindingContext.HttpContext.Request.QueryString;

            if (queryString.HasValue)
            {
                var qsValue = queryString.ToString().TrimStart('?');

                var searchParameters = qsValue
                    .Split('&')
                    .Select(s => s.Split('='))
                    .Where(values => values.Length == 2)
                    .ToLookup(
                        keySelector: values => values[0],
                        elementSelector: values => values[1]);

                if (searchParameters is not null)
                    bindingContext.Result = ModelBindingResult.Success(new SearchParameters() { Parameters = searchParameters });
            }

            return Task.CompletedTask;
        }
    }
}
