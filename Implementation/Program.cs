using Lexer;
using Lexer.Csv;

namespace Implementation;

internal static class Program
{
    internal static string TargetFile;

    internal static CsvSettings CsvSettings = new()
    {
        Separator = ',',
        CommentStarter = '#',
        Patches = false,
    };
    
    internal static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("No arguments given!"); 
            Environment.Exit(1);
        }

        TargetFile = Path.Combine(args[0], "");
        
        Console.WriteLine($"Preparing to read file: {TargetFile}");

        CsvLexer lexer = new CsvLexer(TargetFile, CsvSettings);
        string[] lines = lexer.Test();
        int l = lines.Length;
        for (int i = 0; i < l; i++)
        {
            string line = lines[i];
            Console.WriteLine($"{i+1}: {line}");
        }
    }
}

