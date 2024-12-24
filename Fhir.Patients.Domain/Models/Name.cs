using System.ComponentModel.DataAnnotations;

namespace Fhir.Patients.Domain.Models
{
    public class Name
    {
        public required string Id { get; init; }

        public string Use { get; init; } = string.Empty;
        
        public required string Family { get; init; }

        public string[] Given { get; init; } = [];

        public override string ToString()
            => $"{nameof(Id)}[{Id}];{nameof(Use)}[{Use}];{nameof(Family)}[{Family}];{nameof(Given)}[{string.Join(',', Given)}]";
    }
}
