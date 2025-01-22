using EnvelopeGenerator.Core.Interfaces;
using EnvelopeGenerator.Core.Models;

namespace EnvelopeGenerator.Core.Services;

public class EnvelopeGenerator : IEnvelopeGenerator
{
    private const int BATCH_SIZE = 1000;
    
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnvelopeRepository _repository;
    private readonly IFileGenerator _fileGenerator;

    public EnvelopeGenerator(
        IConnectionFactory connectionFactory,
        IEnvelopeRepository repository,
        IFileGenerator fileGenerator)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _fileGenerator = fileGenerator ?? throw new ArgumentNullException(nameof(fileGenerator));
    }

    public async Task<bool> GenerateEnvelopes(string odbcName, EnvelopeParams parameters)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection(odbcName);
            
            // Get envelope structure and system parameters
            var structure = await _repository.GetEnvelopeStructureAsync(parameters.EnvelopeType);
            var systemParams = await _repository.GetSystemParametersAsync();

            // Get voucher data
            var voucherData = await _repository.GetVoucherDataAsync(parameters);

            // Process data in batches
            foreach (var batch in voucherData.Chunk(BATCH_SIZE))
            {
                await _fileGenerator.ProcessVoucherBatch(
                    batch, 
                    parameters,
                    structure,
                    systemParams);
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log error
            // TODO: Add proper logging
            Console.WriteLine($"Error generating envelopes: {ex.Message}");
            return false;
        }
    }
}