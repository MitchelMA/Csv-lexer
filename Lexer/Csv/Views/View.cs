namespace Lexer.Csv.Views;

public class View
{
    internal readonly object[] Values;
    private int _startIdx;
    private int _len;

    public int StartIdx
    {
        get => _startIdx;
        set
        {
            if (value > Values.Length - 1)
            {
                _startIdx = Values.Length - 1;
                return;
            }

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
            if (value == -1 || _startIdx + value > Values.Length)
            {
                _len = Values.Length - _startIdx;
                return;
            }

            if (value <= 0)
            {
                _len = 1;
                return;
            }

            _len = value;
        }
    }

    public int EndIdx => _startIdx + _len - 1;

    public View(object[] values, int startIdx, int length)
    {
        Values = values;
        StartIdx = startIdx;
        Length = length;
    }

    public View(object[] values, int startIdx)
    {
        Values = values;
        StartIdx = startIdx;
        _len = values.Length;
    }

    public View(object[] values)
    {
        Values = values;
        _startIdx = 0;
        _len = values.Length;
    }
}