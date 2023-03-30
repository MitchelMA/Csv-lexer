namespace Lexer.Csv.Views;

public class View<T> : View
{
    internal readonly T[] Values;
    public int StartIdx
    {
        get => PStartIdx;
        set
        {
            if (value > Values.Length - 1)
            {
                PStartIdx = Values.Length - 1;
                return;
            }

            int diff = value - PStartIdx;
            PStartIdx = value;
            PLen -= diff;
        }
    }
    public int Length
    {
        get => PLen;
        set
        {
            if (value == -1 || PStartIdx + value > Values.Length)
            {
                PLen = Values.Length - PStartIdx;
                return;
            }

            if (value < 0)
            {
                PLen = 0;
                return;
            }

            PLen = value;
        }
    }

    public View(T[] values, int startIdx, int length)
    {
        Values = values;
        StartIdx = startIdx;
        Length = length;
    }

    public View(T[] values, int startIdx)
    {
        Values = values;
        StartIdx = startIdx;
        PLen = values.Length;
    }

    public View(T[] values)
    {
        Values = values;
        PStartIdx = 0;
        PLen = Values.Length;
    }

    public IReadOnlyList<T> GetValues() =>
        Values.AsReadOnly();
    
}