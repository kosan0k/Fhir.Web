namespace Fhir.Patients.Domain.Models
{
    public class Patient : IResource
    {
        public string Id => Name.Id;

        public required Name Name { get; init; }

        public required DateTime BirthDate { get; init; }

        public Gender Gender { get; init; } = Gender.Unknown;

        public bool Active { get; init; } = true;

        public override string ToString()
            => $"{nameof(Id)}[{Id}];{nameof(Name)}[{Name}];{nameof(BirthDate)}[{BirthDate}];{nameof(Active)}[{Active}]";
    }
}
