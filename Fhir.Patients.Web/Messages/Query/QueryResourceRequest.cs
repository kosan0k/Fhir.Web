﻿using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Models;
using MediatR;

namespace Fhir.Patients.Web.Messages.Query
{
    public class QueryResourceRequest<TResource>(SearchParameters? searchParameters) : IRequest<QueryResourceResponse<TResource>>
        where TResource : IResource
    {
        public SearchParameters? SearchParameters { get; } = searchParameters;
    }
}
