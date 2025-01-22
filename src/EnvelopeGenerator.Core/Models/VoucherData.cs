namespace EnvelopeGenerator.Core.Models;

public class VoucherData
{
    public long Mspkod { get; set; }
    public long? ManaHovNum { get; set; }
    public int MtfNum { get; set; }
    public bool ShovarMsp { get; set; }
    public string? Miun { get; set; }
    public int? UniqNum { get; set; }
    public bool Shnati { get; set; }
    
    // Additional fields from shovarhead/shovarlines will be added dynamically
    // based on the envelope structure definition
    public Dictionary<string, object> DynamicFields { get; set; } = new();
}