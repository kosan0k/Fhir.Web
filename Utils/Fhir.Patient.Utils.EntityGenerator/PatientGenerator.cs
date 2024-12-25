using Fhir.Patients.Domain.Models;

namespace Fhir.Patient.Utils.EntityGenerator
{
    internal static class PatientGenerator
    {
        private static readonly Random _random = new();

        private static readonly List<string> _firstNames =
        [
            "John", "Jane", "Michael", "Emily", "William", "Sophia",
            "James", "Isabella", "Robert", "Olivia", "David", "Emma"
        ];

        private static readonly List<string> _middleNames =
        [
            "Alexander", "Grace", "Marie", "Elizabeth", "Joseph", "Andrew",
            "Charlotte", "Rose", "Liam", "Noah", "Benjamin", "Lucas"
        ];

        private static readonly List<string> _lastNames =
        [
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia",
            "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez"
        ];

        internal static Patients.Domain.Models.Patient GenerateRandomPatient()
        {
            return new Patients.Domain.Models.Patient
            {
                Name = new Name
                {
                    Id = Guid.NewGuid().ToString(),
                    Use = "official",
                    Family = GetRandomElement(_lastNames),
                    Given = 
                    [
                        GetRandomElement(_firstNames),
                        GetRandomElement(_middleNames)
                    ]
                },
                Gender = GenerateRandomGender(),
                BirthDate = GenerateRandomDate(DateTime.Now.AddDays(-14), DateTime.Now),
                Active = _random.Next(0, 2) == 1
            };
        }

        private static string GetRandomElement(List<string> list)
        {
            return list[_random.Next(list.Count)];
        }

        private static Gender GenerateRandomGender()
        {
            var genders = Enum.GetValues<Gender>();
            return (Gender)genders.GetValue(_random.Next(genders.Length))!;
        }

        private static DateTime GenerateRandomDate(DateTime start, DateTime end)
        {
            int range = (end - start).Days;
            return start.AddDays(_random.Next(range)).AddSeconds(_random.Next(0, 86400));
        }
    }
}
