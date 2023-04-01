using System.Text;

namespace Lexer.Csv.Views;

public class ByteView : View<byte>
{
    private readonly StringBuilder _sb = new();
    private char[]? _asChars;

    public ByteView(byte[] values, int startIdx, int length, HashSet<int> skips) : base(values, startIdx, length, skips)
    {
    }

    public ByteView(byte[] values, int startIdx, int length) : base(values, startIdx, length)
    {
    }

    public ByteView(byte[] values, int startIdx) : base(values, startIdx)
    {
    }

    public ByteView(byte[] values) : base(values)
    {
    }

    public override string ToString()
    {
        if (SkippedIndices.Count == 0)
            return Encoding.Default.GetString(Values, PStartIdx, PLen);

        _sb.Clear();
        _asChars ??= Encoding.Default.GetChars(Values);

        Span<int> skips = SkippedIndices.Order().ToArray();
        int l = skips.Length;
        int tmpStart = PStartIdx;
        int tmpLen;
        for (int i = 0; i < l; i++)
        {
            tmpLen = skips[i] - tmpStart;
            if(tmpLen < 0)
                continue;
            Collect(_sb, tmpStart, tmpLen);

            tmpStart = skips[i]+1;
        }

        tmpLen = PLen - tmpStart;
        Collect(_sb, tmpStart, tmpLen + StartIdx);

        return _sb.ToString();
    }

    private void Collect(StringBuilder sb, int start, int len)
    {
        for (int i = start; i < start + len; i++)
        {
            sb.Append(_asChars![i]);
        }
    }

    public void TrimStart()
    {
        for (int i = PStartIdx; i < PStartIdx + PLen; i++)
        {
            char ch = (char)Values[i];
            if (char.IsWhiteSpace(ch))
            {
                StartIdx++;
                continue;
            }

            break;
        }
    }

    public void TrimEnd()
    {
        for (int i = EndIdx; i > PStartIdx; i--)
        {
            char ch = (char)Values[i];
            if (char.IsWhiteSpace(ch))
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
}