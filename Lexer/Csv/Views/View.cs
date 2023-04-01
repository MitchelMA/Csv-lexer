namespace Lexer.Csv.Views;

public class View
{
    protected int PStartIdx;
    protected int PLen;
    protected readonly HashSet<int> PSkippedIndices = new();

    public HashSet<int> SkippedIndices => PSkippedIndices;

    public View()
    {
    }

    public View(HashSet<int> skips)
    {
        PSkippedIndices = new(skips);
    }

    public bool AddSkip(int position)
    {
        return PSkippedIndices.Add(position);
    }

    public bool RemoveSkip(int position)
    {
        return PSkippedIndices.Remove(position);
    }

    public int EndIdx => PStartIdx + PLen - 1;
}