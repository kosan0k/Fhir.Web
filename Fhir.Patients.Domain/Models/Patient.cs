namespace Fhir.Patients.Domain.Models
{
    public class Patient(Name name, DateTime birthDate) : IResource
    {
        public string Id => Name.Id;

        public Name Name { get; init; } = name;

        public DateTime BirthDate { get; init; } = birthDate;

        public bool Active { get; init; } = true;

        public override string ToString()
            => $"{nameof(Id)}[{Id}];{nameof(Name)}[{Name}];{nameof(BirthDate)}[{BirthDate}];{nameof(Active)}[{Active}]";
    }
}
