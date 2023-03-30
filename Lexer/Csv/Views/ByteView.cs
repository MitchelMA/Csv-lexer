using System.Text;

namespace Lexer.Csv.Views;

public class ByteView : View<byte>
{
    private string? _out;
    
    public ByteView(byte[] values, int startIdx, int length) : base(values, startIdx, length)
    {
    }

    public ByteView(byte[] values, int startIdx) : base(values, startIdx)
    {
    }

    public ByteView(byte[] values) : base(values)
    {
    }

    public override string ToString() =>
       _out ??=  Encoding.Default.GetString(Values, StartIdx, Length);

    public void TrimStart()
    {
        _out = null;
        for (int i = PStartIdx; i < PLen; i++)
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
        _out = null;
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