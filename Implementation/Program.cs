using Lexer;

namespace Implementation;

internal static class Program
{
    internal static string TargetFile;

    internal static CsvSettings CsvSettings = new()
    {
        Separator = ',',
        CommentStarter = '#',
        Patches = true,
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

        CsvLexer lexer = new CsvLexer(TargetFile);
        string[] lines = lexer.Test()!;
        foreach(var line in lines)
            Console.WriteLine(line);
    }
}

