using System.Text;

namespace Lexer.Csv.Views;

public class ByteView : View<byte>
{
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

    public override string ToString() =>
       Encoding.Default.GetString(Values, StartIdx, Length);

    public void TrimStart()
    {
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