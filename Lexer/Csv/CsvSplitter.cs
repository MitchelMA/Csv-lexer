﻿using System.Text;
using Lexer.Csv.Enums;

namespace Lexer.Csv;

internal class CsvSplitter : IDisposable
{
    private Stream _lineStream;
    private string _splitBuffer = string.Empty;
    private int _lastChar = 0;
    private string[]? _header;

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
            {
                _lineStream.Dispose();
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(_lines[_currentLineIdx]);
            _lineStream = new MemoryStream(bytes);
        }
    }

    internal string CurrentLine => _lines[_currentLineIdx];

    internal string[]? Header => _header;

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
        while (CurrentLineIdx < _lines.Length)
            _splitLines.Add(SplitLine());

        if (_settings.FirstIsHeader)
        {
            _header = _splitLines[0];
            _splitLines.RemoveAt(0);
        }

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
            if (_lastChar == '"')
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
        while ((_lastChar = _lineStream.ReadByte()) != -1)
        {
            if (_lastChar == '"')
            {
                int t = SeekByte();
                if (t == -1)
                {
                    _EOSProc();
                    _lastChar = -1;
                    _currentMode = LexModi.Default;
                    return false;
                }

                if (t != '"')
                {
                    _currentMode = LexModi.Default;
                    return true;
                }

                _lastChar = _lineStream.ReadByte();
            }

            _splitBuffer += (char)_lastChar;
        }

        return false;
    }

    private int SeekByte()
    {
        int c = _lineStream.ReadByte();
        if (c != -1)
            _lineStream.Position--;

        return c;
    }

    #region IDisposable pattern

    private void ReleaseUnmanagedResources()
    {
    }

    private void ReleaseManagedResources()
    {
        _lineStream.Dispose();
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            ReleaseManagedResources();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CsvSplitter()
    {
        Dispose(false);
    }

    #endregion
}