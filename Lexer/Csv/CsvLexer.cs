using System.Text;
using Lexer.Helpers;

namespace Lexer.Csv;

public class CsvLexer
{
    private readonly FileInfo _file;
    private readonly CsvSettings _settings = CsvSettings.Default;
    private string[]? _lines;
    private string[][]? _splits;
    private string[]? _header;

    public string[]? Lines => _lines;
    public string[][]? Splits => _splits;
    public string[]? Header => _header;

    public CsvLexer(string filePath)
    {
        _file = new FileInfo(PathHelper.ToAbsoluteDomain(filePath));
        if (!_file.Exists)
        {
            throw new FileNotFoundException($"File {_file.FullName} does not exist");
        }
    }

    public CsvLexer(string filepath, CsvSettings settings) : this(filepath)
    {
        _settings = settings;
    }

    private string[] GetLines()
    {
        string txt = File.ReadAllText(_file.FullName);
        byte[] bytes = Encoding.UTF8.GetBytes(txt);

        using MemoryStream stream = new(bytes);
        CsvLiner liner = new(stream, _settings);

        return liner.GetLines();
    }

    public string[][] Lex()
    {
        _lines = GetLines();
        using CsvSplitter splitter = new(_lines, _settings);
        _splits = splitter.Split();

        _header = splitter.Header;
        return _splits;
    }

    public Task<string[][]> LexAsync()
    {
        return Task.Run(Lex);
    }
}