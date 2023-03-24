namespace Lexer.Csv.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CsvParameterAttribute : Attribute
{
   internal string? PropertyName;
   
   public CsvParameterAttribute()
   {}

   public CsvParameterAttribute(string propertyName)
   {
      PropertyName = propertyName;
   }
}