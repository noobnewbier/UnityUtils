using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityUtils
{
    public interface ICircularBuffer<T>
    {
        int Count { get; }
        int Capacity { get; set; }
        bool IsBeginning { get; }
        bool IsEnd { get; }
        T Current { get; }
        T Enqueue(T item);
        T Dequeue();
        void Clear();
        T MoveNext();
        T MovePrevious();
    }

    public class CircularBuffer<T> : ICircularBuffer<T>, IEnumerable<T>
    {
        private T[] _buffer;
        private int _currentIndex;
        private int _head;
        private int _tail;

        public CircularBuffer(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "must be positive");

            _buffer = new T[capacity];
            _head = capacity - 1;
        }

        public CircularBuffer(params T[] buffer)
        {
            _buffer = buffer;
            _head = buffer.Length - 1;
            Count = buffer.Length;
        }

        private int WrappedIndex
        {
            get
            {
                var wrappedIndex = ToWrappedIndex(_currentIndex);
                return wrappedIndex;
            }
        }

        public int Count { get; private set; }

        public int Capacity
        {
            get => _buffer.Length;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "must be positive");

                if (value == _buffer.Length) return;

                var buffer = new T[value];
                var count = 0;
                while (Count > 0 && count < value)
                    buffer[count++] = Dequeue();

                _buffer = buffer;
                Count = count;
                _head = count - 1;
                _tail = 0;
            }
        }

        public T Current => _buffer[WrappedIndex];

        public bool IsBeginning => WrappedIndex == 0;

        public bool IsEnd => WrappedIndex == Capacity - 1;


        public T Enqueue(T item)
        {
            _head = (_head + 1) % Capacity;
            var overwritten = _buffer[_head];
            _buffer[_head] = item;
            if (Count == Capacity) _tail = (_tail + 1) % Capacity;
            else ++Count;

            return overwritten;
        }

        public T Dequeue()
        {
            if (Count == 0) throw new InvalidOperationException("queue exhausted");

            var dequeued = _buffer[_tail];
            _buffer[_tail] = default;
            _tail = (_tail + 1) % Capacity;
            --Count;
            return dequeued;
        }

        public void Clear()
        {
            _head = Capacity - 1;
            _tail = 0;
            Count = 0;
        }


        public T MoveNext()
        {
            _currentIndex++;
            return Current;
        }

        public T MovePrevious()
        {
            _currentIndex--;
            return Current;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var tempIndex = _currentIndex;
            for (var i = 0; i < Count; i++)
            {
                yield return _buffer[ToWrappedIndex(tempIndex)];
                tempIndex++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int ToWrappedIndex(int i)
        {
            var index = i % Capacity;
            var wrappedIndex = index < 0 ?
                index + Capacity :
                index;
            return wrappedIndex;
        }
    }
}