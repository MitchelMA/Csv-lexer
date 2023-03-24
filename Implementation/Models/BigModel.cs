using Lexer.Csv.Attributes;

namespace Implementation.Models;

public class BigModel
{
   [CsvPropertyName("cases")]
   public long Cases { get; init; }
   [CsvPropertyName("date")] 
   public DateTime Date { get; init; }
   [CsvPropertyName("deaths")]
   public long Deaths { get; init; }

   public override string ToString() =>
       $"{Date}: {Cases};{Deaths}";
}