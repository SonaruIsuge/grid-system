
using UnityEngine;

public interface IGridTile
{
    int XIndex { get; }
    int YIndex { get; }
    Vector3 WorldPos { get; }
}