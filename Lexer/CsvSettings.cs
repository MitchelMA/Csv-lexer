namespace Lexer;

public struct CsvSettings
{
    public char Separator;
    public char? CommentStarter;
    public bool Patches;

    public static CsvSettings Default => new()
    {
        Separator = ',',
        CommentStarter = null,
        Patches = false,
    };
}