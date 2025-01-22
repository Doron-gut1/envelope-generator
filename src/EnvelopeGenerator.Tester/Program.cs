using EnvelopeGenerator.Core.Interfaces;
using EnvelopeGenerator.Core.Models;
using EnvelopeGenerator.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EnvelopeGenerator.Tester;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Get generator
        var generator = serviceProvider.GetRequiredService<IEnvelopeGenerator>();

        try
        {
            // Create test parameters
            var parameters = new EnvelopeParams
            {
                ActionType = GetUserInput("Enter Action Type (1-Current, 2-Debt, 3-Combined): ", 1, 3),
                EnvelopeType = GetUserInput("Enter Envelope Type: ", 1, 99),
                BatchNumber = GetUserInput("Enter Batch Number: ", 1, 999999),
                IsYearly = GetYesNoInput("Is Yearly? (Y/N): "),
                FamilyCode = GetOptionalLongInput("Enter Family Code (optional, press Enter to skip): "),
                ClosureNumber = GetOptionalIntInput("Enter Closure Number (optional, press Enter to skip): "),
                VoucherGroup = GetOptionalLongInput("Enter Voucher Group (optional, press Enter to skip): "),
                OutputDirectory = GetDirectoryPath("Enter Output Directory: ")
            };

            // Get ODBC name
            Console.Write("Enter ODBC Name: ");
            string odbcName = Console.ReadLine() ?? "";

            // Generate envelopes
            Console.WriteLine("\nGenerating envelopes...");
            bool success = await generator.GenerateEnvelopes(odbcName, parameters);

            // Show result
            if (success)
            {
                Console.WriteLine("Envelopes generated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to generate envelopes.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IEnvelopeGenerator, Core.Services.EnvelopeGenerator>();
        services.AddTransient<IFileGenerator, FileGenerator>();
        services.AddTransient<IEncodingService, HebrewEncodingService>();
        // Add your ODBC connection factory here
        services.AddTransient<IConnectionFactory>(sp => 
            new YourOdbcConnectionFactory()); // You'll need to implement this
        services.AddTransient<IEnvelopeRepository, EnvelopeRepository>();
    }

    private static int GetUserInput(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
            {
                return result;
            }
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    private static bool GetYesNoInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.ToUpper();
            if (input == "Y") return true;
            if (input == "N") return false;
            Console.WriteLine("Please enter Y or N.");
        }
    }

    private static long? GetOptionalLongInput(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : long.Parse(input);
    }

    private static int? GetOptionalIntInput(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : int.Parse(input);
    }

    private static string GetDirectoryPath(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var path = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                return path;
            }
            Console.WriteLine("Please enter a valid directory path.");
        }
    }
}
