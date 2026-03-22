using System;

namespace SonaruUtilities
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }        
    }
}