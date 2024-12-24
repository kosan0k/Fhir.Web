namespace Fhir.Patients.Domain.Models
{
    public class Name(string id, string family)
    {
        public string Id { get; init; } = id;

        public Use Use { get; init; } = Use.Official;

        public string Family { get; init; } = family;

        public string[] Given { get; init; } = [];

        public override string ToString()
            => $"{nameof(Id)}[{Id}];{nameof(Use)}[{Use}];{nameof(Family)}[{Family}];{nameof(Given)}[{string.Join(',', Given)}]";
    }
}
