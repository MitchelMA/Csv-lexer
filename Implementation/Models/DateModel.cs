using Lexer.Csv.Attributes;

namespace Implementation.Models;

public class DateModel
{
    [CsvPropertyName("date1")] public DateTime Date1 { get; init; }

    [CsvPropertyName("date2")] public DateTime Date2 { get; init; }

    [CsvPropertyName("date3")] public DateTime Date3 { get; init; }

    public override string ToString() =>
        $"{Date1}\n{Date2}\n{Date3}";
}