﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Implementation.Models;
using Lexer.Csv;
using Lexer.Csv.Views;
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

    internal static void Main2(string[] args)
    {
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

    internal static void Main(string[] args)
    {
        string txt = "dit is een hele mooie string";
        string smoll = "prachtige text, niet?";
        char[] smollC = smoll.ToCharArray();
        byte[] smollB = Encoding.Default.GetBytes(smoll);
        Console.WriteLine(smollB.GetHashCode());
        Console.WriteLine(txt.GetHashCode());

        StringView view = new(smollC, 0, 7);
        StringView view2 = new(smollC, 6, -1);
        view.Trim();
        view2.Trim();
        string output = view;
        string output2 = view2;
        
        Console.WriteLine($":{output}:");
        Console.WriteLine($":{output2}:");
        Console.WriteLine(output.GetHashCode());
        Console.WriteLine(output2.GetHashCode());

        ByteView[] bv = CsvLexer.Test(smollB);
        foreach(var val in bv)
            Console.WriteLine(val);
        Console.WriteLine("end");
    }
}