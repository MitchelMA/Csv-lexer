using System.Text;
using Lexer.Csv.Enums;

namespace Lexer.Csv;

internal class CsvSplitter
{
    private Stream _lineStream;
    private string _splitBuffer = string.Empty;
    private int _lastChar = 0;

    private int _currentLineIdx = 0;
    // The current line that's being worked on
    private List<string> _lineSplits = new();
    // the collection of split lines
    private List<string[]> _splitLines = new();
    
    private string[] _lines;
    private CsvSettings _settings;

    private LexModi _currentMode = LexModi.Default;
    private Dictionary<LexModi, Func<bool>> _modi = new();

    internal int CurrentLineIdx
    {
        get => _currentLineIdx;
        set
        {
            if (value == _currentLineIdx)
                return;

            _currentLineIdx = value;
            if (_currentLineIdx >= _lines.Length)
                return;
            
            _lineStream.Dispose();
            byte[] bytes = Encoding.UTF8.GetBytes(_lines[_currentLineIdx]);
            _lineStream = new MemoryStream(bytes);
        }
    }

    internal string CurrentLine => _lines[_currentLineIdx];

    internal CsvSplitter(string[] lines, CsvSettings settings)
    {
        _lines = lines;
        _settings = settings;
        _lineStream = new MemoryStream(Encoding.UTF8.GetBytes(lines[0]));
        
        ModiPopulator();
    }

    private void ModiPopulator()
    {
        _modi.Clear();
        
        _modi.Add(LexModi.Default, DefaultHandler);
        _modi.Add(LexModi.String, StringHandler);
    }

    internal string[][] Split()
    {
        while(CurrentLineIdx < _lines.Length)
            _splitLines.Add(SplitLine());

        return _splitLines.ToArray();
    }

    private string[] SplitLine()
    {
        _lastChar = 0;
        _lineSplits.Clear();
        
        while (_lastChar != -1)
        {
            _modi[_currentMode]();
        }

        CurrentLineIdx++;
        return _lineSplits.ToArray();
    }

    private void _EOSProc()
    {
        string trimmed = _splitBuffer.Trim();
        _lineSplits.Add(trimmed);
        _splitBuffer = "";
    }

    private bool DefaultHandler()
    {
        while ((_lastChar = _lineStream.ReadByte()) != -1)
        {
            if (ToStringModus(_lastChar))
            {
                _currentMode = LexModi.String;
                return true;
            }

            if (_lastChar == _settings.Separator)
            {
                _EOSProc();
                continue;
            }

            _splitBuffer += (char)_lastChar;
        }
        
        _EOSProc();

        return false;
    }

    private bool StringHandler()
    {
        throw new NotImplementedException();
    }

    private bool ToStringModus(int c) => c == '"';
}