using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static OMC.Util.Helpers;

namespace OMC.Util {
    public class Deque<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection {

        private T[] elements;

        private int head;

        private int tail;

        private int count;

        public int Count {
            get {
                return count;
            }
        }

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public int Capacity => elements.Length;

        public Deque() {
            elements = new T[1];
        }

        public Deque(IEnumerable<T> collection) {
            T[] array = collection.ToArray();
            if (array.Length <= 0) {
                throw new ArgumentException("IEnumerable must have at least one element");
            }
            elements = array;
            count = array.Length;
            tail = array.Length - 1;
        }

        public Deque(int capacity) {
            if (capacity <= 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            elements = new T[capacity];
        }

        // Goes from head to tail (front to back)
        public IEnumerator<T> GetEnumerator() {
            return new DequeueEnum<T>(elements, head, tail);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public bool Contains(T element) {
            int stop = head + count;
            for (int i = head; i < stop; i++) {
                if (EqualityComparer<T>.Default.Equals(element, elements[i % elements.Length])) {
                    return true;
                }
            }
            return false;
        }

        public void Clear() {
            int stop = head + count;
            for (int i = head; i < stop; i++) {
                elements[i % elements.Length] = default;
            }
            head = 0;
            tail = 0;
            count = 0;
        }

        public void CopyTo(Array array, int index) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }

            CopyTo((T[])array, index);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (arrayIndex >= array.Length || arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            int offset = 0;
            int stop = head + count;
            for (int i = head; i < stop; i++) {
                array[arrayIndex + offset] = elements[i % elements.Length];
                offset++;
            }
        }

        public void EnsureCapacity(int capacity) {
            if (elements.Length < capacity) {
                T[] newArray = new T[capacity];
                CopyTo(newArray, 0);
                elements = newArray;
                head = 0;
                tail = count - 1;
            }
        }

        public bool TryPeekFront(out T result) {
            if (count > 0) {
                result = PeekFront();
                return true;
            }
            result = default;
            return false;
        }

        public bool TryPeekBack(out T result) {
            if (count > 0) {
                result = PeekBack();
                return true;
            }
            result = default;
            return false;
        }

        public bool TryDequeueFront(out T result) {
            if (count > 0) {
                result = DequeueFront();
                return true;
            }
            result = default;
            return false;
        }

        public bool TryDequeueBack(out T result) {
            if (count > 0) {
                result = DequeueBack();
                return true;
            }
            result = default;
            return false;
        }

        public T PeekFront() {
            if (count > 0) {
                return elements[head];
            } else {
                throw new InvalidOperationException();
            }
        }

        public T PeekBack() {
            if (count > 0) {
                return elements[tail];
            } else {
                throw new InvalidOperationException();
            }
        }

        public void TrimExcess() {
            if (count < 0.9f * elements.Length) {
                T[] newArray = new T[count];
                CopyTo(newArray, 0);
                elements = newArray;
                head = 0;
                tail = count - 1;
            }
        }

        public void TrimExcess(int capacity) {
            if (count <= capacity && capacity >= 1) {
                T[] newArray = new T[capacity];
                CopyTo(newArray, 0);
                elements = newArray;
                head = 0;
                tail = count - 1;
            } else {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
        }

        public void EnqueueBack(T element) {
            if (count == elements.Length) {
                DoubleArraySize();
            }

            if (count > 0) {
                IncrementTail();
            }

            count++;

            elements[tail] = element;
        }

        public T DequeueBack() {
            T element;
            if (count > 0) {
                element = elements[tail];
                elements[tail] = default;
                if (count > 1) {
                    DecrementTail();
                }
                count--;
                return element;
            } else {
                throw new InvalidOperationException();
            }
        }

        public void EnqueueFront(T element) {
            if (count == elements.Length) {
                DoubleArraySize();
            }

            if (count > 0) {
                DecrementHead();
            }

            count++;

            elements[head] = element;
        }

        public T DequeueFront() {
            T element;
            if (count > 0) {
                element = elements[head];
                elements[head] = default;
                if (count > 1) {
                    IncrementHead();
                }
                count--;
                return element;
            } else {
                throw new InvalidOperationException();
            }
        }

        private void DoubleArraySize() {
            EnsureCapacity(elements.Length * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementHead() {
            head = Mod(head + 1, elements.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DecrementHead() {
            head = Mod(head - 1, elements.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementTail() {
            tail = Mod(tail + 1, elements.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DecrementTail() {
            tail = Mod(tail - 1, elements.Length);
        }
    }

    public class DequeueEnum<T> : IEnumerator<T> {

        private T[] elements;

        private int startPosition;

        private int position;

        private int startRemaining;

        private int remaining;

        public DequeueEnum(T[] elements, int head, int tail) {
            this.elements = elements;
            startPosition = head - 1;
            position = startPosition;
            startRemaining = Mod(tail - head + elements.Length, elements.Length) + 1;
            remaining = startRemaining;
        }

        public bool MoveNext() {
            position++;
            if (position == elements.Length) {
                position -= elements.Length;
            }
            remaining--;
            return remaining >= 0;
        }

        public void Reset() {
            position = startPosition;
            remaining = startRemaining;
        }

        public void Dispose() { }

        object IEnumerator.Current {
            get {
                return Current;
            }
        }

        public T Current {
            get {
                try {
                    return elements[position];
                } catch (IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }

        T IEnumerator<T>.Current {
            get {
                return Current;
            }
        }
    }
}
