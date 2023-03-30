namespace Lexer.Csv.Views;

public class StringView : View<char>
{
    private string? _out;

    public StringView(string values, int startIdx, int length) : base(values.ToCharArray(), startIdx, length)
    {
    }

    public StringView(string values, int startIdx) : base(values.ToCharArray(), startIdx)
    {
    }

    public StringView(string values) : base(values.ToCharArray())
    {
    }

    public StringView(char[] values, int startIdx, int length) : base(values, startIdx, length)
    {
    }

    public StringView(char[] values, int startIdx) : base(values, startIdx)
    {
    }

    public StringView(char[] values) : base(values)
    {
    }

    public override string ToString() =>
        _out ??= new(Values, StartIdx, Length);

    public void TrimStart()
    {
        _out = null;
        for (int i = PStartIdx; i < PLen; i++)
        {
            if (char.IsWhiteSpace(Values[i]))
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
            if (char.IsWhiteSpace(Values[i]))
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

    public static implicit operator string(StringView v) => v.ToString();
}