using System.Text;
using Lexer.Csv.Attributes;

namespace Implementation.Models;

public class TestModel
{
    [CsvPropertyName("jaar")]
    public int Jaar { get; init; }
    [CsvPropertyName("merk")]
    public string Merk { get; init; }
    [CsvPropertyName("type")]
    public string Type { get; init; }
    [CsvPropertyName("omschrijving")]
    public string? Omschrijving { get; init; }
    [CsvPropertyName("prijs")]
    public float Prijs { get; init; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Jaar: {Jaar}");
        sb.AppendLine($"Merk: {Merk}");
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"Omschrijving: {Omschrijving}");
        sb.Append($"Prijs: {Prijs}");
        return sb.ToString();
    }
}