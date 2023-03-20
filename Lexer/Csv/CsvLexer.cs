using System.Text;
using Lexer.Csv.Streams;

namespace Lexer.Csv;

public class CsvLexer : IDisposable
{
    private readonly FileInfo? _file;
    private readonly CsvSettings _settings = CsvSettings.Default;
    private bool _sDisposed = false;

    private StrippedStream _sStream;

    private string[]? _lines;
    private string[][]? _splits;
    private string[]? _header;

    public string[]? Lines => _lines;
    public string[][]? Splits => _splits;
    public string[]? Header => _header;

    public bool IsDisposed { get; private set; } = false;

    public CsvLexer(string fileText)
    {
        _sStream = new(Encoding.UTF8.GetBytes(fileText));
    }

    public CsvLexer(FileInfo file)
    {
        if (!file.Exists)
            throw new FileNotFoundException($"File {file.FullName} does not exist");

        _file = file;
        _sStream = new(File.ReadAllText(file.FullName));
    }

    public CsvLexer(string fileText, CsvSettings settings) : this(fileText)
    {
        _settings = settings;
    }

    public CsvLexer(FileInfo file, CsvSettings settings) : this(file)
    {
        _settings = settings;
    }

    private string[] GetLines()
    {
        CsvLiner liner = new(_sStream, _settings);
        string[] lines = liner.GetLines();
        _sStream.Reset();

        return lines;
    }

    public string[][] Lex()
    {
        if (_splits is not null)
            return _splits;
        
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

    #region IDisposable pattern

    private void ReleaseManagedResources()
    {
        _sStream.Dispose();
        _sDisposed = true;
    }

    private void ReleaseUnmanagedResources()
    {
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            ReleaseManagedResources();
        }

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CsvLexer()
    {
        Dispose(false);
    }

    #endregion
}