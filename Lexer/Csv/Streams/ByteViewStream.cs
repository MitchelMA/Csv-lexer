using System.Text;
using Lexer.Csv.Views;

namespace Lexer.Csv.Streams;

internal class ByteViewStream
{
    private ByteView _view;
    private int _value;

    internal int Position => _view.StartIdx + _view.Length;

    internal ByteViewStream(byte[] bytes)
    {
        _view = new(bytes, 0, 0);
    }

    internal ByteViewStream(string text) : this(Encoding.UTF8.GetBytes(text))
    {
    }

    internal ByteView SnapShot() =>
        new(_view.Values, _view.StartIdx, _view.Length);

    /// <summary>
    /// Skips past all the current bytes in view
    /// </summary>
    /// <returns>
    /// True - when it skipped past the end
    /// False - when it still has bytes to consume
    /// </returns>
    internal bool Skip()
    {
        _view.StartIdx += _view.Length;
        return _view.StartIdx + _view.Length == _view.Values.Length;
    }

    internal int Consume()
    {
        if (Position > _view.Values.Length - 1)
            return -1;
        
        _value = _view.Values[Position];
        _view.Length++;

        return _value;
    }

    internal int Peek()
    {
        if (Position > _view.Values.Length - 1)
            return -1;
        
        return _view.Values[Position];
    }
}