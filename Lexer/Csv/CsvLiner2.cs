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
      curLine.Trim();
      _bvStream.Skip();
      if (_settings.Patches && curLine.Length == 0)
         return false;
      
      _lines.Add(curLine);
      return true;
   }

   private bool DefaultHandler()
   {
      while ((_lastChar = _bvStream.Peek()) != -1)
      {
         if (_lastChar == '\n')
         {
            _bvStream.AddSkipAtCurrentPosition();
            _EOLProc();
         }

         if (_lastChar == ',')
         {
            _bvStream.AddSkipAtCurrentPosition();
            _EOLProc();
         }

         if (_lastChar == '"')
            _bvStream.AddSkipAtCurrentPosition();
            
         _bvStream.Consume();
      }

      _EOLProc();

      return false;
   }

   private bool StringHandler()
   {
      throw new NotImplementedException();
   }

   private bool CommentHandler()
   {
      throw new NotImplementedException();
   }
}