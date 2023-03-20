using System.Text;

namespace Lexer.Csv.Streams;

internal class StrippedStream : IDisposable
{
    private long _position = 0;
    private byte[] _bytes;

    internal long Length => _bytes.Length;
    
    internal long Position => _position;

    internal int Previous
    {
        get
        {
            if (_position == 0)
                return -1;

            return _bytes[_position - 1];
        }
    }

    internal int Current
    {
        get
        {
            if (Surpassed)
                return -1;

            return _bytes[_position];
        }
    }

    internal int Next
    {
        get
        {
            if (_position + 1 >= Length)
                return -1;

            return _bytes[_position + 1];
        }
    }

    private bool Surpassed => _position >= Length;

    internal bool IsDisposed { get; private set; } = false;

    internal StrippedStream(IEnumerable<byte> bytes)
    {
        _bytes = bytes.ToArray();
    }

    internal StrippedStream(IEnumerable<char> chars)
    {
        _bytes = chars.Select(x => (byte)x).ToArray();
    }

    internal StrippedStream(string text)
    {
        _bytes = Encoding.UTF8.GetBytes(text);
    }

    internal int ReadByte()
    {
        int b = Current;
        MoveNext();
        return b;
    }

    internal int Peek() => Current;

    internal void Reset()
    {
        _position = 0;
    }

    private void MoveNext()
    {
        _position++;
    }

    #region IDisposable Pattern

    private void ReleaseManagedResources()
    {
        _position = 0;
        _bytes = null!;
    }

    private void ReleaseUnmanagedResources()
    {
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            ReleaseManagedResources();
        }

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~StrippedStream()
    {
        Dispose(false);
    }

    #endregion
}