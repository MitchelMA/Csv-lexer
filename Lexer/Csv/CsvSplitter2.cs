using Lexer.Csv.Enums;
using Lexer.Csv.Streams;
using Lexer.Csv.Views;

namespace Lexer.Csv;

internal class CsvSplitter2
{
    private ByteViewStream _bvs;
    private int _lastChar = 0;
    private ByteView[]? _header;

    private int _currentLineIdx = 0;
    
    // The current line that's being worked on
    private readonly List<ByteView?> _lineSplits = new();
    
    // The collection of split lines
    private readonly List<ByteView[]> _splitLines = new();

    private readonly ByteView[] _lines;
    private readonly CsvSettings _settings;

    private LexModi _currentMode = LexModi.Default;
    private readonly Dictionary<LexModi, Func<bool>> _modi = new();

    internal int CurrentLineIdx
    {
        get => _currentLineIdx;
        private set
        {
            if (value == _currentLineIdx)
                return;
            
            // Todo! create dispose for ByteViewStream
            // _bvs.Dispose()
            _currentLineIdx = value;

            if (_currentLineIdx >= _lines.Length)
                return;

            _bvs = new(_lines[_currentLineIdx]);
        }
    }
    
    internal ByteView[]? Header => _header;

    internal CsvSplitter2(ByteView[] lines, CsvSettings settings)
    {
        _lines = lines;
        _settings = settings;
        _bvs = new(lines[0]);
        
        ModiPopulator();
    }

    private void ModiPopulator()
    {
        _modi.Clear();
        
        _modi.Add(LexModi.Default, DefaultHandler);
        _modi.Add(LexModi.String, StringHandler);
    }

    internal ByteView[][] Split()
    {
        while(CurrentLineIdx < _lines.Length)
            _splitLines.Add(SplitLine());

        if (_settings.FirstIsHeader)
        {
            _header = _splitLines[0];
            _splitLines.RemoveAt(0);
        }

        return _splitLines.ToArray();
    }

    private ByteView?[] SplitLine()
    {
        _lastChar = 0;
        _lineSplits.Clear();

        while (_lastChar != -1)
            _modi[_currentMode]();

        CurrentLineIdx++;
        return _lineSplits.ToArray();
    }

    private void _EOSProc()
    {
        ByteView cap = _bvs.Capture();
        _bvs.Skip();
        cap.Trim();
        _lineSplits.Add(cap.Length > 0 ? cap : null);
    }

    private bool DefaultHandler()
    {
        while ((_lastChar = _bvs.Peek()) != -1)
        {
            if (_lastChar == '"')
            {
                _currentMode = LexModi.String;
                return true;
            }

            if (_lastChar == _settings.Separator)
            {
                _bvs.AddSkipAtCurrentPosition();
                _EOSProc();
            }

            _bvs.Consume();
        }
        
        _EOSProc();

        return false;
    }

    private bool StringHandler()
    {
        throw new NotImplementedException();
    }
}