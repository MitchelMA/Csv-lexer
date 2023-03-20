using System.Text;

namespace Lexer.Csv;

public class CsvLexer : IDisposable
{
    private readonly FileInfo? _file;
    private readonly string? _filestring;
    private readonly CsvSettings _settings = CsvSettings.Default;
    private StreamReader _sReader;
    private bool _sDisposed = false;

    private string[]? _lines;
    private string[][]? _splits;
    private string[]? _header;

    public string[]? Lines => _lines;
    public string[][]? Splits => _splits;
    public string[]? Header => _header;

    public bool IsDisposed { get; private set; } = false;

    public CsvLexer(string fileText)
    {
        _filestring = fileText;
        MemoryStream ms = new(Encoding.UTF8.GetBytes(_filestring));
        _sReader = new(ms);
    }

    public CsvLexer(FileInfo file)
    {
        if (!file.Exists)
            throw new FileNotFoundException($"File {file.FullName} does not exist");

        _file = file;
        _sReader = _file.OpenText();
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
        if (_sDisposed && !IsDisposed)
        {
            _sReader = GetStream();
        }
        CsvLiner liner = new(_sReader, _settings);
        string[] lines = liner.GetLines();
        if (_settings.ImmediateClosing)
        {
            _sReader.Dispose();
            _sDisposed = true;
        }
        else
        {
            _sReader.DiscardBufferedData();
            _sReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        return lines;
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

    private StreamReader GetStream()
    {
        StreamReader? sr = _file?.OpenText();

        if (sr is null)
        {
            using MemoryStream ms = new(Encoding.UTF8.GetBytes(_filestring!));
            sr = new(ms);
        }


        return sr;
    }

    #region IDisposable pattern

    private void ReleaseManagedResources()
    {
        _sReader.Dispose();
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