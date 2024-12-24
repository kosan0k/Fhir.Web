using Fhir.Patients.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fhir.Patients.Web.Models
{
    public class PatientDto
    {
        public string Id => Name.Id;

        [Required]
        public required NameDto Name { get; init; }

        [Required]
        public required DateTime BirthDate { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter<Gender>))]
        public Gender Gender { get; init; } = Gender.Unknown;

        public bool Active { get; init; } = true;

        public Patient ToDomain()
            => new()
            {
                Name = Name.ToDomain(),
                BirthDate = BirthDate,
                Active = Active,
                Gender = Gender,
            };
    }
}
