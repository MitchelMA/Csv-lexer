using System.Diagnostics;
using System.Runtime.InteropServices;
using Implementation.Models;
using Lexer.Csv;
using Lexer.Helpers;

namespace Implementation;

internal static class Program
{
    private static readonly string[] Files =
    {
        "./csv/dates.csv",
        "./csv/simple.csv",
        "./csv/test.csv",
        "./csv/big.csv"
    };

    private static readonly CsvSettings CsvSettings = new()
    {
        Separator = ',',
        CommentStarter = '#',
        Patches = true,
        FirstIsHeader = true,
    };

    [DllImport("Dll1.dll")]
    private static extern void Test(string charArr);

    internal static void Main(string[] args)
    {
        IntPtr val = CsvLexer.Create(10);
        Console.WriteLine(CsvLexer.GetX(val));
        Test("string as char-array");
        return;
        
        int idx = 0;
        foreach (var fileName in Files)
        {
            FileInfo info = new(PathHelper.ToAbsoluteDomain(fileName));

            using CsvLexer lexer = new CsvLexer(info, CsvSettings);

            var stopwatch = Stopwatch.StartNew();
            lexer.Lex();
            stopwatch.Stop();
            Console.WriteLine($"Took {stopwatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"Line-count: {lexer.Lines!.Length}");
            Console.WriteLine($"{GC.GetTotalMemory(true) * 9.537E-7f}mb usage\n");
            var vals = lexer.Splits!;

            for (int i = 0; i < lexer.Header?.Length; i++)
            {
                var cur = lexer.Header?[i];
                Console.Write($"{cur}\t");
            }

            Console.WriteLine();

            int l = vals.Length > 10 ? 10 : vals.Length;

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < vals[i].Length; j++)
                {
                    var cur = vals[i][j];
                    Console.Write($"`{cur}`\t");
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nDeserialized:");
            object[] model = null!;
            switch (idx)
            {
                case 0:
                {
                    model = lexer.Deserialize<DateModel>();
                }
                    break;

                case 1:
                {
                    model = lexer.Deserialize<SimpleModel>();
                }
                    break;

                case 2:
                {
                    model = lexer.Deserialize<TestModel>();
                }
                    break;

                case 3:
                {
                    model = lexer.Deserialize<BigModel>();
                }
                    break;
            }

            int innerIdx = 0;
            foreach (var value in model)
            {
                if (innerIdx >= 9)
                    break;
                
                Console.WriteLine($"{value}\n");
                innerIdx++;
            }

            Console.WriteLine("------------------------\n");
            idx++;
        }

        Console.ReadKey();
    }
}