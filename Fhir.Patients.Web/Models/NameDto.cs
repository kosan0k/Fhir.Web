using Fhir.Patients.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Fhir.Patients.Web.Models
{
    public class NameDto
    {
        public string Id { get; init; } = string.Empty;

        public string Use { get; init; } = string.Empty;

        [Required]
        public string Family { get; init; } = string.Empty;

        public string[] Given { get; init; } = [];

        public Name ToDomain()
            => new()
            {
                Id = Id,
                Family = Family,
                Use = Use,
                Given = Given,
            };
    }
}
