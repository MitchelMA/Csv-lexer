namespace Lexer.Csv.Views;

public class View
{
    protected int PStartIdx;
    protected int PLen;
    protected readonly HashSet<int> SkippedIndices = new();

    public bool AddSkip(int position)
    {
        return SkippedIndices.Add(position);
    }

    public bool RemoveSkip(int position)
    {
        return SkippedIndices.Remove(position);
    }

    public int EndIdx => PStartIdx + PLen - 1;
}