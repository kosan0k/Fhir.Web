using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Fhir.Patient.Utils.EntityGenerator;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Started");

        var apiUrl = args.Length != 0
            ? args[0]
            : "http://localhost:5076/patients";

        using HttpClient httpClient = new();

        if(await IsServiceAvailable(httpClient, apiUrl))
            await SendRandomPatientsAsync(httpClient, apiUrl);
        else
            Console.WriteLine("Service is unavailable after multiple attempts.");

        Console.WriteLine("Done");
        Console.ReadLine();
    }

    private static async Task SendRandomPatientsAsync(HttpClient httpClient, string apiUrl)
    {
        var serializationOptions = new JsonSerializerOptions { WriteIndented = true };
        serializationOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        for (int i = 0; i < 100; i++)
        {
            var patient = PatientGenerator.GenerateRandomPatient();

            string patientJson = JsonSerializer.Serialize(patient, serializationOptions);
            StringContent content = new(patientJson, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine($"Patient sent successfully");
                else
                    Console.WriteLine($"Failed to send patient. Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending patient: {ex.Message}");
            }
        }
    }

    private static async Task<bool> IsServiceAvailable(HttpClient httpClient, string apiUrl)
    {
        const int maxRetries = 5;
        int retryDelayMs = 2000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine($"Pinging service (attempt {attempt})...");
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Service is available.");
                    return true;
                }
            }
            catch
            {
                // Ignoring exceptions during ping.
            }

            Console.WriteLine($"Service unavailable. Retrying in {retryDelayMs / 1000} seconds...");
            await Task.Delay(retryDelayMs);

            retryDelayMs *= 2;
        }

        return false;
    }
}

