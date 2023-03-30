namespace Lexer.Csv.Views;

public class View<T> : View
{
    internal new readonly T[] Values;

    public View(T[] values, int startIdx, int length) : base((values as object[])!, startIdx, length)
    {
        Values = values;
    }

    public View(T[] values, int startIdx) : base((values as object[])!, startIdx)
    {
        Values = values;
    }

    public View(T[] values) : base((values as object[])!)
    {
        Values = values;
    }
}