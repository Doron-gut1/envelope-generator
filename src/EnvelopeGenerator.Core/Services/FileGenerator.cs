using EnvelopeGenerator.Core.Models;
using System.Text;

namespace EnvelopeGenerator.Core.Services;

public class FileGenerator : IFileGenerator
{
    private readonly IEncodingService _encodingService;

    public FileGenerator(IEncodingService encodingService)
    {
        _encodingService = encodingService;
    }

    public async Task ProcessVoucherBatch(
        IEnumerable<VoucherData> voucherBatch,
        EnvelopeParams parameters,
        EnvelopeStructure structure,
        SystemParameters systemParams)
    {
        var files = new Dictionary<string, StreamWriter>();
        try
        {
            InitializeFiles(files, parameters);

            foreach (var voucher in voucherBatch)
            {
                var line = FormatLine(voucher, structure, systemParams);
                
                // Write to appropriate file based on action type and voucher type
                if (parameters.ActionType == 3 && voucher.UniqNum == -100)
                {
                    await files["SvrMeshulavShotefHov"].WriteLineAsync(line);
                }
                else if (voucher.ManaHovNum == 0)
                {
                    await files["SvrShotef"].WriteLineAsync(line);
                }
                else
                {
                    await files["SvrHov"].WriteLineAsync(line);
                }
            }
        }
        finally
        {
            foreach (var file in files.Values)
            {
                await file.DisposeAsync();
            }
        }
    }

    private void InitializeFiles(Dictionary<string, StreamWriter> files, EnvelopeParams parameters)
    {
        var mntName = parameters.BatchNumber.ToString().Replace("/", "");
        var baseDir = parameters.OutputDirectory;

        if (parameters.ActionType == 1 || parameters.ActionType == 2)
        {
            var fileName = parameters.ActionType == 1 ? "SvrShotef" : "SvrHov";
            files[fileName] = new StreamWriter(Path.Combine(baseDir, $"{fileName}_{mntName}.txt"), false, Encoding.GetEncoding(1255));
        }
        else
        {
            files["SvrShotef"] = new StreamWriter(Path.Combine(baseDir, $"SvrShotef_{mntName}.txt"), false, Encoding.GetEncoding(1255));
            files["SvrHov"] = new StreamWriter(Path.Combine(baseDir, $"SvrHov_{mntName}.txt"), false, Encoding.GetEncoding(1255));
            files["SvrMeshulavShotefHov"] = new StreamWriter(Path.Combine(baseDir, $"SvrMeshulavShotefHov_{mntName}.txt"), false, Encoding.GetEncoding(1255));
        }
    }

    private string FormatLine(VoucherData voucher, EnvelopeStructure structure, SystemParameters systemParams)
    {
        var sb = new StringBuilder();

        foreach (var field in structure.Fields.OrderBy(f => f.Order))
        {
            string value = GetFieldValue(voucher, field);
            value = FormatFieldValue(value, field, structure, systemParams);
            sb.Append(value);
        }

        return sb.ToString();
    }

    private string GetFieldValue(VoucherData voucher, FieldDefinition field)
    {
        if (voucher.DynamicFields.TryGetValue(field.Name, out var value))
        {
            return value?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    private string FormatFieldValue(string value, FieldDefinition field, EnvelopeStructure structure, SystemParameters systemParams)
    {
        // Handle encoding if needed
        if (field.Type == FieldType.Text && structure.DosHebrewEncoding)
        {
            value = _encodingService.ConvertToDosHebrew(value);
        }

        if (systemParams.RevHeb && field.Type == FieldType.Text)
        {
            value = _encodingService.ReverseHebrew(value);
        }

        // Handle padding and length
        switch (field.Type)
        {
            case FieldType.Text:
                return value.PadRight(field.Length);
                
            case FieldType.Numeric:
                return value.PadLeft(field.Length, '0');
                
            case FieldType.Currency:
                var format = $"F{structure.NumOfDigits}";
                if (decimal.TryParse(value, out var number))
                {
                    return number.ToString(format).PadLeft(field.Length);
                }
                return "".PadLeft(field.Length, '0');
                
            default:
                return "".PadRight(field.Length);
        }
    }
}