namespace EnvelopeGenerator.Core.Models;

public class EnvelopeStructure
{
    public List<FieldDefinition> Fields { get; set; } = new();
    public int NumOfDigits { get; set; }
    public int PositionOfShnati { get; set; }
    public bool DosHebrewEncoding { get; set; }
    public int NumOfPerutLines { get; set; }
    public int NumOfPerutFields { get; set; }
}

public class FieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public int Length { get; set; }
    public int Order { get; set; }
    public DataSource Source { get; set; }
}

public enum FieldType
{
    Text = 1,
    Numeric = 2,
    Currency = 3
}

public enum DataSource
{
    ShovarHead = 1,
    ShovarLines = 2,
    Dynamic = 4,
    ShovarHeadNx = 5
}