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
        string smoll = "prachtige text, niet?   ";
        byte[] smollB = Encoding.Default.GetBytes(smoll);

        ByteView bv = new(smollB)
        {
            StartIdx = 9
        };
        // dit is allemaal nummer manipulation (het creeërt geen nieuwe string)
        bv.AddSkip(10);
        bv.AddSkip(13);
        // dit creeërt wel een nieuwe string
        Console.WriteLine($":{bv}:");
        // maar de Trim() niet
        bv.Trim();
        // en logischerwijs hier weer wel
        Console.WriteLine($":{bv}:");
        
    }
}