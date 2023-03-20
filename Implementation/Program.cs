using System.Diagnostics;
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

        var stopwatch = Stopwatch.StartNew();
        CsvLexer lexer = new CsvLexer(TargetFile, CsvSettings);
        stopwatch.Stop();
        
        Console.WriteLine($"Took {stopwatch.Elapsed.TotalMilliseconds}ms");
        string[] lines = lexer.Test();
        int l = lines.Length;
        for (int i = 0; i < l; i++)
        {
            string line = lines[i];
            Console.WriteLine($"{i+1}: {line}");
        }

        var vals = lexer.Test2();
        for (int i = 0; i < vals.Length; i++)
        {
            for (int j = 0; j < vals[i].Length; j++)
            {
                var cur = vals[i][j];
                Console.Write(cur + " ");
            }
            Console.WriteLine();
        }
    }
}

