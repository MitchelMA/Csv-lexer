namespace Lexer.Csv.Views;

public class View
{
    protected int PStartIdx;
    protected int PLen;

    public int EndIdx => PStartIdx + PLen - 1;
}