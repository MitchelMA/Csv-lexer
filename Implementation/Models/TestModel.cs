using Lexer.Csv.Attributes;

namespace Implementation.Models;

public struct TestModel
{
    [CsvParameter("Test1")]
    public int Test1 { get; set; }
    
    [CsvParameter("Test2")]
    public int Test2 { get; set; }
    
    [CsvParameter("Test3")]
    public int Test3 { get; set; }
}