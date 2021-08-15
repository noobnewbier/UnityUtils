using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityUtils
{
    public interface ITwoWayEnumerator<T> : IEnumerator<T>
    {
        bool MovePrevious();
    }

    public class TwoWayEnumerator<T> : ITwoWayEnumerator<T>
    {
        private readonly List<T> _buffer;
        private readonly IEnumerator<T> _enumerator;
        private int _index;

        public TwoWayEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _buffer = new List<T>();
            _index = -1;
        }

        public bool MovePrevious()
        {
            if (_index < 0) return false;

            --_index;
            return true;
        }

        public bool MoveNext()
        {
            if (_index < _buffer.Count - 1)
            {
                ++_index;
                return true;
            }

            if (_enumerator.MoveNext())
            {
                _buffer.Add(_enumerator.Current);
                ++_index;
                return true;
            }

            return false;
        }

        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _buffer.Count)
                    throw new InvalidOperationException("index out of bounds!: " + _index + ", Count: " + _buffer.Count);

                return _buffer[_index];
            }
        }

        public void Reset()
        {
            _enumerator.Reset();
            _buffer.Clear();
            _index = -1;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        object IEnumerator.Current => Current;
    }
}