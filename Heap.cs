using System;
using System.Collections.Generic;
using System.Linq;

public interface IHeapNode
{
	float GetTotalCost();
}

/// <summary>
/// A collection to get an item with smallest cost on top.
/// Float based.
/// Based on https://www.c-sharpcorner.com/article/binary-heap-in-c-sharp/
/// with some improvements.
/// </summary>
public class MinHeap<T> where T : IHeapNode, IEquatable<T>
{
	private T[] _array;
	public int Size { get; private set; }

	public MinHeap(int size)
	{
		_array = new T[size + 1];
		Size = 0;
	}
	public MinHeap(int size, IEnumerable<T> list) : this(size)
	{
		foreach (var item in list)
			Insert(item);
	}

	public void Insert(T val)
	{
		if (_array is null || Size < 0)
			throw new System.Exception("MinHeap was not initialized properly. Size is 0.");
		_array[++Size] = val;
		HeapifyBottomToTop(Size);
	}

	private void HeapifyBottomToTop(int index)
	{
		if (index <= 1)
			return;

		var parentIndex = index / 2;

		if (_array[index].GetTotalCost() < _array[parentIndex].GetTotalCost())
		{
			// Tuple based swap.
			(_array[index], _array[parentIndex])
				= (_array[parentIndex], _array[index]);
		}
		HeapifyBottomToTop(parentIndex);
	}

	public T Pop()
	{
		if (_array is null || Size == 0)
			throw new System.Exception("MinHeap was not initialized properly. Size is 0.");

		var topValue = _array[1];
		_array[1] = _array[Size];
		Size--;

		HeapifyTopToBottom(1);

		return topValue;
	}

	private void HeapifyTopToBottom(int index)
	{
		var leftIndex = index * 2;
		var rightIndex = leftIndex + 1;
		if (Size < leftIndex) // No children.
			return;

		if (Size == leftIndex) // One child.
		{
			if (_array[index].GetTotalCost() > _array[leftIndex].GetTotalCost())
			{
				(_array[index], _array[leftIndex]) =
					(_array[leftIndex], _array[index]);
			}
			return;
		}
		// Two children.
		var smallestChildIndex =
			_array[leftIndex].GetTotalCost() < _array[rightIndex].GetTotalCost() ?
			leftIndex : rightIndex;

		if (_array[index].GetTotalCost() > _array[smallestChildIndex].GetTotalCost())
		{
			(_array[index], _array[smallestChildIndex]) =
				(_array[smallestChildIndex], _array[index]);
		}
		HeapifyTopToBottom(smallestChildIndex);
	}

	public T Peek() => Size == 0 ? default(T) : _array[1]; // Right?

	public bool Contains(T item) => _array.Contains(item);

	public void Update(T item)
	{
		var index = 0;
		for (int i = 1; i < _array.Length; i++)
		{
			if (ReferenceEquals(_array[i], item)
				|| _array[i].Equals(item))
			{
				index = i;
				break;
			}
		}

		(_array[1], _array[index]) = (_array[index], _array[1]);
		HeapifyTopToBottom(1);
		HeapifyBottomToTop(index); // Not sure how much this will help, but just to be sure
	}
}
