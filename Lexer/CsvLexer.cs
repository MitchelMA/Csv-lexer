using Lexer.Helpers;
using Lexer.Modus;

namespace Lexer;

public class CsvLexer
{
    private readonly FileInfo _file;
    private readonly CsvSettings _settings = CsvSettings.Default;

    private delegate bool ModusDelegate(ref string buffer, ICollection<string> lines, Stream s);

    private Dictionary<PreLexModus, ModusDelegate> _preLexMethods = new();
    private PreLexModus _currentModus = PreLexModus.Default;


    public CsvLexer(string filePath)
    {
        _file = new FileInfo(PathHelper.ToAbsoluteDomain(filePath));
        if (!_file.Exists)
        {
            throw new FileNotFoundException($"File {_file.FullName} does not exist");
        }

        PopulateDict();
    }

    public CsvLexer(string filepath, CsvSettings settings)
    {
        _file = new FileInfo(PathHelper.ToAbsoluteDomain(filepath));
        if (!_file.Exists)
        {
            throw new FileNotFoundException($"File {_file.FullName} does not exist");
        }

        _settings = settings;
        PopulateDict();
    }

    private void PopulateDict()
    {
        _preLexMethods.Add(PreLexModus.Default, DefaultHandler);
        _preLexMethods.Add(PreLexModus.String, StringHandler);
        _preLexMethods.Add(PreLexModus.Comment, CommentHandler);
    }

    #region PreLexing

    /// <summary>
    /// The first iteration: going through the file, and parsing out the comments if enabled and
    /// parsing the lines.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string[]? PreLex(FileStream fs)
    {
        string buffer = string.Empty;
        List<string> lines = new();
        bool available = true;

        while (available)
        {
            available = _preLexMethods[_currentModus](ref buffer, lines, fs);
        }

        return lines.ToArray();
    }

    private bool _EOLproc(ref string buffer, ICollection<string> lines)
    {
        string trimmed = buffer.Trim();
        if (_settings.Patches && trimmed.Length == 0)
            return false;

        lines.Add(trimmed);
        buffer = "";
        return true;
    }

    private bool DefaultHandler(ref string buffer, ICollection<string> lines, Stream stream)
    {
        int c;

        while ((c = stream.ReadByte()) != -1)
        {
            if (ToCommentModus(c))
            {
                _currentModus = PreLexModus.Comment;
                return true;
            }

            if (ToStringModus(c))
            {
                _currentModus = PreLexModus.String;
                return true;
            }


            if (c == '\n')
            {
                _EOLproc(ref buffer, lines);
                continue;
            }

            buffer += (char)c;
        }

        _EOLproc(ref buffer, lines);

        return false;
    }

    private bool StringHandler(ref string buffer, ICollection<string> lines, Stream stream)
    {
        throw new NotImplementedException();
    }

    private bool CommentHandler(ref string buffer, ICollection<string> lines, Stream stream)
    {
        // keep consuming till the end of the line
        int c;
        while ((c = stream.ReadByte()) is not '\n' and not -1) ;
        switch (c)
        {
            case '\n':
                _EOLproc(ref buffer, lines);
                break;
            case -1:
                _EOLproc(ref buffer, lines);
                return false;
        }
        _currentModus = PreLexModus.Default;
        return true;
    }

    private bool ToStringModus(int c)
    {
        if (c == '"')
            return true;

        return false;
    }

    private bool ToCommentModus(int c)
    {
        if (_settings.CommentStarter is null)
            return false;

        if (c == _settings.CommentStarter)
            return true;

        return false;
    }

    #endregion

    public string[]? Test()
    {
        using FileStream stream = _file.OpenRead();
        int c;
        Console.WriteLine("Binary Value");
        while ((c = stream.ReadByte()) != -1)
        {
            Console.Write(c);
        }
            
        Console.WriteLine("\n----\n");

        stream.Position = 0;
        return PreLex(stream);
    }
}