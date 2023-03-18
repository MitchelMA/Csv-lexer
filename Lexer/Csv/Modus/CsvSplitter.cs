namespace Lexer.Csv.Modus;

internal class CsvSplitter
{
    private string[] _lines;
    private CsvSettings _settings;

    internal CsvSplitter(string[] lines, CsvSettings settings)
    {
        _lines = lines;
        _settings = settings;
    }
}