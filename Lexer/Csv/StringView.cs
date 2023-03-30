using System.Runtime.InteropServices;

namespace Lexer.Csv;

public struct StringView
{
    internal readonly string Str;
    private int _startIdx;
    private int _len;

    private string? _output;

    public string Reference => Str;
    public int StartIdx
    {
        get => _startIdx;
        set
        {
            if (value > Str.Length - 1)
                return;

            int diff = value - _startIdx;
            _startIdx = value;
            _len -= diff;
        }
    }
    public int Length
    {
        get => _len;
        set
        {
            if (value <= 0)
                return;

            if (_startIdx + value > Str.Length)
                return;

            _len = value;
        }
    }
    public int EndIdx => _startIdx + _len - 1;
    
    public StringView(string str, int startIdx, int len)
    {
        Str = str;
        _startIdx = startIdx;
        _len = len;
    }

    public StringView(string str, int startIdx)
    {
        Str = str;
        _startIdx = startIdx;
        _len = Str.Length - _startIdx;
    }

    public StringView(string str)
    {
        Str = str;
        _startIdx = 0;
        _len = Str.Length;
    }

    public override string ToString() =>
        _output ??= Str.Substring(_startIdx, _len);

    public void TrimStart()
    {
        for (int i = _startIdx; i < _len; i++)
        {
            if (char.IsWhiteSpace(Str[i]))
            {
                StartIdx++;
                continue;
            }

            break;
        }
    }

    public void TrimEnd()
    {
        for (int i = EndIdx; i > _startIdx; i--)
        {
            if (char.IsWhiteSpace(Str[i]))
            {
                Length--;
                continue;
            }

            break;
        }
    }
    
    public void Trim()
    {
       TrimStart(); 
       TrimEnd();
    }

    // public static implicit operator string(StringView sv) => sv.ToString();
}