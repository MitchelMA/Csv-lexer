using Lexer.Csv.Enums;
using Lexer.Csv.Streams;
using Lexer.Csv.Views;

namespace Lexer.Csv;

internal class CsvLiner2
{
   private int _lastChar = 0;
   private readonly List<ByteView> _lines = new();
   private LexModi _currentMode = LexModi.Default;
   private readonly CsvSettings _settings;

   private readonly ByteViewStream _bvStream;
   private readonly Dictionary<LexModi, Func<bool>> _modi = new();

   internal CsvLiner2(ByteViewStream stream, CsvSettings settings)
   {
      _bvStream = stream;
      _settings = settings;
      
      ModiPopulator();
   }

   private void ModiPopulator()
   {
      _modi.Clear();
      
      _modi.Add(LexModi.Default, DefaultHandler);
      _modi.Add(LexModi.String, StringHandler);
      _modi.Add(LexModi.Comment, CommentHandler);
   }

   internal ByteView[] GetLines()
   {
      while (_lastChar != -1)
      {
         _modi[_currentMode]();
      }

      return _lines.ToArray();
   }

   private bool _EOLProc()
   {
      ByteView curLine = _bvStream.Capture();
      _bvStream.Skip();
      curLine.Trim();
      if (_settings.Patches && curLine.Length == 0)
         return false;
      
      _lines.Add(curLine);
      return true;
   }

   private bool DefaultHandler()
   {
      while ((_lastChar = _bvStream.Peek()) != -1)
      {
         if (_lastChar == _settings.CommentStarter)
         {
            _currentMode = LexModi.Comment;
            return true;
         }

         if (_lastChar == '"')
         {
            _currentMode = LexModi.String;
            return true;
         }

         if (_lastChar == '\n')
         {
            _bvStream.AddSkipAtCurrentPosition();
            _EOLProc();
         }

         _bvStream.Consume();
      }

      _EOLProc();

      return false;
   }

   private bool StringHandler()
   {
      // consume string-starter `"`
      _bvStream.Consume();
      if (_bvStream.Peek() == '"')
         throw new Exception($"Cannot start string with two double-quotes `\"\"` at {_bvStream.Capture()}:position {_bvStream.Position}");

      while ((_lastChar = _bvStream.Peek()) != -1)
      {
         if (_lastChar == '"')
         {
            _bvStream.Consume();
            int t = _bvStream.Peek();

            if (t == -1)
            {
               _EOLProc();
               _lastChar = -1;
               return false;
            }

            if (t != '"')
            {
               _currentMode = LexModi.Default;
               return true;
            }
         }

         _bvStream.Consume();
      }
      
      throw new Exception($"String never ended with `\"` at {_bvStream.Capture()}:position {_bvStream.Position}");
   }

   private bool CommentHandler()
   {
      while ((_lastChar = _bvStream.Consume()) is not '\n' and not -1)
      {
         _bvStream.AddSkipAtLastPosition();
      }

      _EOLProc();
      if (_lastChar == -1)
         return false;

      _currentMode = LexModi.Default;
      return true;
   }
}