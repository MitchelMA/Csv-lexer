using Lexer.Csv.Attributes;

namespace Implementation.Models;

public class SimpleModel
{
    [CsvPropertyName("test1")]
    public int Test1 { get; init; }
    
    [CsvPropertyName("test2")]
    public int Test2 { get; set; }
    
    [CsvPropertyName("test3")]
    public int Test3 { get; set; }

    public override string ToString() =>
        $"{Test1}, {Test2}, {Test3}";
}