using Lexer.Csv.Attributes;

namespace Implementation.Models;

public class BigModel
{
   [CsvPropertyName("date")] 
   public DateTime Date { get; init; }
   [CsvPropertyName("cases")]
   public long Cases { get; init; }
   [CsvPropertyName("deaths")]
   public long Deaths { get; init; }

   public override string ToString() =>
       $"{Date}: {Cases};{Deaths}";
}