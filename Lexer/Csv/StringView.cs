using System.Runtime.InteropServices;

namespace Lexer.Csv;

public struct StringView
{
    private string _str;
    private int _startIdx;
    private int _len;

    private string? _output;

    public StringView(string str, int startIdx, int len)
    {
        _str = str;
        _startIdx = startIdx;
        _len = len;
    }

    public StringView(string str, int startIdx)
    {
        _str = str;
        _startIdx = startIdx;
        _len = _str.Length - _startIdx;
    }

    public StringView(string str)
    {
        _str = str;
        _startIdx = 0;
        _len = _str.Length;
    }

    public override string ToString() =>
        _output ??= _str.Substring(_startIdx, _len);

    public void Trim()
    {
        int startIdx = _startIdx;
        int len = _len;

        for (int i = startIdx; i < len; i++)
        {
            if (char.IsWhiteSpace(_str[i]))
            {
                startIdx++;
                len--;
                continue;
            }

            break;
        }

        for (int i = startIdx + len-1; i > startIdx; i--)
        {
            if (char.IsWhiteSpace(_str[i]))
            {
                len--;
                continue;
            }

            break;
        }

        _startIdx = startIdx;
        _len = len;
    }

    // public static implicit operator string(StringView sv) => sv.ToString();
}