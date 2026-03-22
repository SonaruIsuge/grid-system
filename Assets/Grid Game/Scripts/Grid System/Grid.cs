
using System;
using SNR_Event;
using UnityEngine;


public enum CornerType
{
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom
}


public class Grid<TGridData> where TGridData : IGridTile
{
    public int RowNumber { get; private set; }
    public int ColumnNumber { get; private set; }
    public float CellSize { get; private set; }
    public Vector3 OffsetPosition;

    public int MaxCellNumber => RowNumber * ColumnNumber;
    public Vector2 GridSize => new Vector2(ColumnNumber * CellSize, RowNumber * CellSize);
    //public Vector3 GridCenter => offsetPosition + new Vector3((GridSize.x - CellSize) / 2, 0, (GridSize.y - CellSize) / 2);
    
    private TGridData[,] gridDataArray;


    public Grid(int row, int column, float cellSize, Vector3 offsetPosition, Func<Grid<TGridData>, int, int, TGridData> createData)
    {
        this.RowNumber = row;
        this.ColumnNumber = column;
        this.CellSize = cellSize;
        this.OffsetPosition = offsetPosition;

        gridDataArray = new TGridData[ColumnNumber, RowNumber];

        for (var x = 0; x < ColumnNumber; x++)
        {
            for (var y = 0; y < RowNumber; y++)
            {
                SetData(x, y, createData(this, x, y));
            }
        }
    }


    public bool CheckCellExist(int xIndex, int yIndex)
    {
        return xIndex >= 0 && xIndex < ColumnNumber && yIndex >= 0 && yIndex < RowNumber;
    }


    public Vector3 GetWorldPosition(int xIndex, int yIndex)
    {
        return new Vector3(xIndex, 0, yIndex) * CellSize + OffsetPosition;
    }


    public Vector3 GetCellCorner(int xIndex, int yIndex, CornerType cornerType)
    {
        var centerPos = GetWorldPosition(xIndex, yIndex);
        
        switch (cornerType)
        {
            case CornerType.LeftTop:
                return centerPos + new Vector3(-CellSize / 2, 0, CellSize / 2);
            case CornerType.LeftBottom:
                return centerPos + new Vector3(-CellSize / 2, 0, -CellSize / 2);
            case CornerType.RightTop:
                return centerPos + new Vector3(CellSize / 2, 0, CellSize / 2);
            case CornerType.RightBottom:
                return centerPos + new Vector3(CellSize / 2, 0, -CellSize / 2);
            default:
                return centerPos;
        }
    }


    public Vector2Int GetGridIndex(Vector3 worldPosition)
    {
        var result = Vector2Int.zero;
        result.x = Mathf.RoundToInt((worldPosition - OffsetPosition).x / CellSize);
        result.y = Mathf.RoundToInt((worldPosition - OffsetPosition).z / CellSize);
        return result;
    }


    public void SetData(int xIndex, int yIndex, TGridData data)
    {
        if (!CheckCellExist(xIndex, yIndex))
            return;

        gridDataArray[xIndex, yIndex] = data;
        EventManager.RaiseEvent<OnGridDataChanged<TGridData>>(new OnGridDataChanged<TGridData>(this, xIndex, yIndex, data));
    }


    public void SetData(Vector3 worldPosition, TGridData data)
    {
        var gridIndex = GetGridIndex(worldPosition);
        SetData(gridIndex.x, gridIndex.y, data);
    }


    public TGridData GetData(int xIndex, int yIndex)
    {
        if (!CheckCellExist(xIndex, yIndex))
            return default(TGridData);

        return gridDataArray[xIndex, yIndex];
    }


    public TGridData GetData(Vector3 worldPosition)
    {
        var gridIndex = GetGridIndex(worldPosition);

        return GetData(gridIndex.x, gridIndex.y);
    }
}
