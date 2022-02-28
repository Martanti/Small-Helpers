public class Array2D<T>
{
    private T[] _array;
    public uint MaxX { get; private set; }
    public uint MaxY { get; private set; }

    private uint GetIndex(uint x, uint y) => x + y * MaxX;
    public void SetAt(uint x, uint y, T value) =>
        _array[GetIndex(x, y)] = value;
    public T GetAt(uint x, uint y) => _array[GetIndex(x, y)];
    public Array2D(uint maxX, uint maxY)
    {
        _array = new T[maxX * maxY];
        MaxX = maxX;
        MaxY = maxY;
    }
}