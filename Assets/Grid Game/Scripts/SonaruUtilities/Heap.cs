using UnityEngine.UIElements;

namespace SonaruUtilities
{
    /// <summary>
    /// An implementation of a fixed-size binary max-heap using an array as the underlying storage structure.
    /// - The generic type T must implement IHeapItem<T> (including the HeapIndex property and the CompareTo method).
    /// - Provides O(log n) operations for Add, RemoveFirst, and UpdateItem.
    /// - The Contains method can determine whether an item exists in O(1) time (by comparing `item.HeapIndex` with the array index).
    /// - The maximum capacity (`maxHeapSize`) must be specified during construction; the heap size is fixed and will not automatically expand or contract.
    /// - Not thread-safe; manual synchronization is required when used in a multithreaded environment.
    /// </summary>
    
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] items;
        private int currentItemCount;

        public int Count => currentItemCount;


        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }


        public T RemoveFirst()
        {
            var firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }


        public void UpdateItem(T item)
        {
            SortUp(item);
        }
        
        
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }


        public bool Contains(T item)
        {
            return item.HeapIndex < currentItemCount && Equals(items[item.HeapIndex], item);
        }


        public void Clear()
        {
            currentItemCount = 0;
        }


        private void SortDown(T item)
        {
            while (true)
            {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;
                var swapIndex = 0;
                
                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount &&
                        items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                        return;
                }
                else
                    return;
            }            
        }


        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
        }
    }
}