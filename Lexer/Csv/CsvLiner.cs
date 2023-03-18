using System.Data.Common;
using System.Data.Odbc;
using Lexer.Csv.Modus;
using Microsoft.VisualBasic.FileIO;

namespace Lexer.Csv;

internal class CsvLiner
{
    private int _lastChar = 0;
    private string _lineBuffer = string.Empty;
    private readonly List<string> _lines = new();
    private PreLexModus _currentModus = PreLexModus.Default;
    private CsvSettings _settings;

    private bool _forceEnd;
    private Stream _stream;

    private Dictionary<PreLexModus, Func<bool>> _modi = new();

    internal CsvLiner(Stream stream, CsvSettings settings)
    {
        _stream = stream;
        _settings = settings;
        ModiPopulator();
    }

    private void ModiPopulator()
    {
        _modi.Clear();

        _modi.Add(PreLexModus.Default, DefaultHandler);
        _modi.Add(PreLexModus.String, StringHandler);
        _modi.Add(PreLexModus.Comment, CommentHandler);
    }

    internal string[] GetLines()
    {
        while (_lastChar != -1)
        {
            _modi[_currentModus]();
        }

        return _lines.ToArray();
    }

    private bool _EOLProc()
    {
        string trimmed = _lineBuffer.Trim();
        if (_settings.Patches && trimmed.Length == 0)
            return false;

        _lines.Add(trimmed);
        _lineBuffer = "";
        return true;
    }

    private bool DefaultHandler()
    {
        while ((_lastChar = _stream.ReadByte()) != -1)
        {
            if (ToCommentModus(_lastChar))
            {
                _currentModus = PreLexModus.Comment;
                return true;
            }

            if (ToStringModus(_lastChar))
            {
                _currentModus = PreLexModus.String;
                return true;
            }


            if (_lastChar == '\n')
            {
                _EOLProc();
                continue;
            }

            _lineBuffer += (char)_lastChar;
        }

        _EOLProc();

        return false;
    }

    private bool StringHandler()
    {
        _lineBuffer += (char)_lastChar;
        _lastChar = _stream.ReadByte();
        if (_lastChar == '"')
            throw new Exception($"Cannot start string with two double-quotes `\"\"` at {_stream.Position}");

        _lineBuffer += (char)_lastChar;
        while ((_lastChar = _stream.ReadByte()) != -1)
        {
            if (_lastChar == '"')
            {
                _lineBuffer += (char)_lastChar;
                int t = SeekByte();
                if (t == -1)
                {
                    _EOLProc();
                    _lastChar = -1;
                    return false;
                }

                if (t != '"')
                {
                    _currentModus = PreLexModus.Default;
                    return true;
                }

                _lastChar = _stream.ReadByte();
            }

            _lineBuffer += (char)_lastChar;
        }

        throw new Exception($"String never ended with `\"` at {_stream.Position}");
    }

    private bool CommentHandler()
    {
        // keep consuming till the end of the line
        while ((_lastChar = _stream.ReadByte()) is not '\n' and not -1) ;
        switch (_lastChar)
        {
            case '\n':
                _EOLProc();
                break;

            case -1:
                _EOLProc();
                return false;
        }

        _currentModus = PreLexModus.Default;
        return true;
    }

    private bool ToStringModus(int c) => c == '"';
    private bool ToCommentModus(int c) => c == _settings.CommentStarter;

    private int SeekByte()
    {
        int c = _stream.ReadByte();
        _stream.Position--;

        if (c == -1)
            _forceEnd = true;
        return c;
    }
}