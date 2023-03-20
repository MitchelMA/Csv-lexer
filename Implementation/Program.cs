using System.Diagnostics;
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
        FirstIsHeader = true,
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

        var stopwatch = Stopwatch.StartNew();
        var vals = lexer.Lex();
        stopwatch.Stop();
        Console.WriteLine($"Took {stopwatch.Elapsed.TotalMilliseconds}ms");

        for (int i = 0; i < lexer.Header?.Length; i++)
        {
            var cur = lexer.Header?[i];
            Console.Write($"{cur}\t");
        }

        Console.WriteLine();

        for (int i = 0; i < vals.Length; i++)
        {
            for (int j = 0; j < vals[i].Length; j++)
            {
                var cur = vals[i][j];
                Console.Write($"`{cur}`" + " ");
            }

            Console.WriteLine();
        }
    }
}