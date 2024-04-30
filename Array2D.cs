using System;

namespace Array2D
{
    public partial class Array2D<T>
    {
        private readonly T[] _array;
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        public int TotalSpace => MaxX * MaxY;
        private int GetIndex(int x, int y) => x + y * MaxX;

        public void SetAt(int x, int y, T value) =>
            _array[GetIndex(x, y)] = value;

        public T GetAt(int x, int y) => _array[GetIndex(x, y)];

        public Array2D(int maxX, int maxY)
        {
            _array = new T[maxX * maxY];
            MaxX = maxX;
            MaxY = maxY;
        }

        public void SetAll(T value)
        {
            int maxIndex = MaxX * MaxY;
            for (int i = 0; i < maxIndex; i++)
            {
                _array[i] = value;
            }
        }

        public bool IsInBounds(int x, int y) => x >= 0
            && x < MaxX
            && y >= 0
            && y < MaxY;

        public void ForeachValidItem(Action<int, int, T> delegateFunction)
        {
            for (int y = 0; y < MaxY; y++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    T heldValue = GetAt(x, y);
                    if (GetAt(x, y) is null)
                    {
                        continue;
                    }

                    delegateFunction(x, y, heldValue);
                }
            }
        }

        public void ForeachValidItem(Func<int, int, bool> condition, Action<int, int, T> delegateFunction)
        {
            for (int y = 0; y < MaxY; y++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    T heldValue = GetAt(x, y);
                    if (heldValue is null || condition(x, y))
                    {
                        continue;
                    }

                    delegateFunction(x, y, heldValue);
                }
            }
        }
    }
}
