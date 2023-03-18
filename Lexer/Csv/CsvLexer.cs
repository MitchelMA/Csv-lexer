using Lexer.Csv.Modus;
using Lexer.Helpers;

namespace Lexer.Csv;

public class CsvLexer
{
    private readonly FileInfo _file;
    private readonly CsvSettings _settings = CsvSettings.Default;

    public CsvLexer(string filePath)
    {
        _file = new FileInfo(PathHelper.ToAbsoluteDomain(filePath));
        if (!_file.Exists)
        {
            throw new FileNotFoundException($"File {_file.FullName} does not exist");
        }

    }

    public CsvLexer(string filepath, CsvSettings settings)
    {
        _file = new FileInfo(PathHelper.ToAbsoluteDomain(filepath));
        if (!_file.Exists)
        {
            throw new FileNotFoundException($"File {_file.FullName} does not exist");
        }

        _settings = settings;
    }

    public string[] Test()
    {
        using FileStream stream = _file.OpenRead();
        CsvLiner liner = new(stream, _settings);
        int c;
        Console.WriteLine("Binary Value");
        while ((c = stream.ReadByte()) != -1)
        {
            Console.Write(c);
        }
            
        Console.WriteLine("\n----\n");

        stream.Position = 0;
        return liner.GetLines();
    }
}