using System.Diagnostics;
using Lexer.Csv;
using Lexer.Helpers;

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
        FileInfo info = new(PathHelper.ToAbsoluteDomain(TargetFile));
        // string text = File.ReadAllText(PathHelper.ToAbsoluteDomain(TargetFile));
        
        using CsvLexer lexer = new CsvLexer(info, CsvSettings);

        var stopwatch = Stopwatch.StartNew();
        string[][] vals = null!;
        vals = lexer.Lex();

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
                Console.Write($"`{cur}`\t");
            }

            Console.WriteLine();
        }
    }
}