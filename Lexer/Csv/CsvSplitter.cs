using Lexer.Csv.Enums;

namespace Lexer.Csv;

internal class CsvSplitter
{
    private string _splitBuffer = string.Empty;
    private int _lastChar = 0;
    // The current line that's being worked on
    private List<string> _lineSplits = new();
    // the collection of split lines
    private List<string[]> _splitLines = new();
    
    private string[] _lines;
    private CsvSettings _settings;

    private LexModi _currentMode = LexModi.Default;
    private Dictionary<LexModi, Func<bool>> _modi = new();


    internal CsvSplitter(string[] lines, CsvSettings settings)
    {
        _lines = lines;
        _settings = settings;
        
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
        Span<string> lines = _lines;
        int l = lines.Length;
        for (int i = 0; i < l; i++)
        {
            var cur = lines[i];
            _splitLines.Add(SplitLine(cur));
        }

        return _splitLines.ToArray();
    }

    private string[] SplitLine(string line)
    {
        _lineSplits.Clear();
        
        while (_lastChar != -1)
        {
            _modi[_currentMode]();
        }
        
        return _lineSplits.ToArray();
    }

    private bool DefaultHandler()
    {
        throw new NotImplementedException();
    }

    private bool StringHandler()
    {
        throw new NotImplementedException();
    }
}