namespace EnvelopeGenerator.Core.Models;

public class EnvelopeParams
{
    /// <summary>
    /// Type of action: 1-Current only, 2-Debt only, 3-Current+Debt
    /// </summary>
    public int ActionType { get; set; }

    /// <summary>
    /// Type of envelope (from mivnemtf table)
    /// </summary>
    public int EnvelopeType { get; set; }

    /// <summary>
    /// Batch number
    /// </summary>
    public int BatchNumber { get; set; }

    /// <summary>
    /// Is yearly envelope
    /// </summary>
    public bool IsYearly { get; set; }

    /// <summary>
    /// Family code (optional)
    /// </summary>
    public long? FamilyCode { get; set; }

    /// <summary>
    /// Closure number (optional)
    /// </summary>
    public int? ClosureNumber { get; set; }

    /// <summary>
    /// Voucher group (optional)
    /// </summary>
    public long? VoucherGroup { get; set; }

    /// <summary>
    /// Name of the calling form
    /// </summary>
    public string FormName { get; set; } = string.Empty;

    /// <summary>
    /// Output directory for generated files
    /// </summary>
    public string OutputDirectory { get; set; } = string.Empty;
}