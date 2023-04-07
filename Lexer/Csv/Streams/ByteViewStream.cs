﻿using System.Text;
using Lexer.Csv.Views;

namespace Lexer.Csv.Streams;

internal class ByteViewStream
{
    private ByteView _view;
    private int _capacity;

    internal int Length
    {
        get => _view.Length;
        set
        {
            if (value > _capacity)
            {
                _view.Length = _capacity;
                return;
            }

            _view.Length = value;
        }
    }
    internal int Capacity
    {
        get => _capacity;
        set
        {
            if (value == -1 || _view.StartIdx + value > _view.Values.Length)
            {
                _capacity = _view.Values.Length - _view.StartIdx;
                return;
            }

            if (value < -1)
            {
                _capacity = 0;
                return;
            }

            _capacity = value;
        }
    }
    internal int Position => _view.StartIdx + _view.Length;

    internal int AbsCapacityPos => _view.StartIdx + Capacity;
    
    internal ByteViewStream(byte[] bytes)
    {
        _view = new(bytes, 0, 0);
        _capacity = _view.Values.Length;
    }

    internal ByteViewStream(string text) : this(Encoding.UTF8.GetBytes(text))
    {
    }

    internal ByteViewStream(ByteView bv)
    {
        _view = new(bv.Values, bv.StartIdx, 0, bv.SkippedIndices);
        _capacity = bv.Length;
    }

    internal ByteView Capture() =>
        new(_view.Values, _view.StartIdx, _view.Length, _view.SkippedIndices);

    /// <summary>
    /// Skips past all the current bytes in view
    /// </summary>
    /// <returns>
    /// True - when it skipped past the end
    /// False - when it still has bytes to consume
    /// </returns>
    internal bool Skip()
    {
        _view.StartIdx += Length;
        return _view.StartIdx + _view.Length >= _view.Values.Length;
    }

    internal int Consume()
    {
        if (Position > AbsCapacityPos - 1 || Position > _view.Values.Length - 1)
            return -1;
        
        var value = _view.Values[Position];
        Length++;

        return value;
    }

    internal int Peek()
    {
        if (Position > AbsCapacityPos - 1 || Position > _view.Values.Length - 1)
            return -1;
        
        return _view.Values[Position];
    }

    internal bool AddSkidIndex(int index) =>
        _view.AddSkip(index);

    internal bool AddSkipAtCurrentPosition() =>
        _view.AddSkip(Position);
    
    internal bool AddSkipAtLastPosition()
    {
        if (Length == 0)
            return false;

        return _view.AddSkip(Position - 1);
    }
}