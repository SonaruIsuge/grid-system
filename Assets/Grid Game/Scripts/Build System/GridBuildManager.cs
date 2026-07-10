
using System;
using System.Collections.Generic;
using System.Linq;
using SNR_Event;
using SNR_PathFinding;
using UnityEngine;
using UtilSNR.Common;
using UtilSNR.Pool;

namespace SNR_BuildSystem
{
    public enum ItemFacing
    {
        Up = 0,
        Left = 90,
        Down = 180,
        Right = 270
    }

    [Serializable]
    public struct PlaceItemData
    {
        public int ItemID;
        public int XIndex;
        public int YIndex;
        public ItemFacing Facing;
    }
    
    public class GridBuildManager : TSceneSingletonBehaviour<GridBuildManager>
    {
        private class PlacedItemRecord
        {
            public TiledPlaceable Instance;
            public PlaceItemData Data;
            public List<PathFindableTile> Tiles;
        }
        
        [SerializeField] private TiledItemList tiledItemList;
        [SerializeField] private Transform placedItemRoot;

        public TiledItemList TiledItemList => tiledItemList;

        private GameBoard Board => GameManager.Instance != null ? GameManager.Instance.GameBoard : null;
        private Grid<PathFindableTile> Grid => Board != null? Board.Grid : null;
        private readonly Dictionary<Vector2Int, PlacedItemRecord> placedItemRegistry = new();

        public PlaceableData GetTiledItemData(int id)
        {
            var item = tiledItemList.GetItemById(id);
            
            return item ? item.Data : null;
        }

        public void PlaceTiledItem(TiledPlaceable item, int xIndex, int yIndex,  ItemFacing itemFacing)
        {
            if (!Board || !item)
                return;

            var itemCellSize = WorldSizeToCellSize(item.Width, item.Height);
            var rotatedCellSize = GetRotatedItemCellSize(itemCellSize, itemFacing);
            
            if(!CheckTilesPlaceable(xIndex, yIndex, rotatedCellSize.x, rotatedCellSize.y))
                return;

            var initItemPos = GetRotatedPlaceItemPos(item, xIndex, yIndex, itemFacing);
            var placedItem = PoolManager.Instance.Spawn(
                item, 
                initItemPos, 
                Quaternion.Euler(0, (int)itemFacing, 0), 
                placedItemRoot
            );

            if (!placedItem)
                return;

            // Record placed item data
            var placeData = new PlaceItemData
            {
                ItemID = item.Data.ID,
                XIndex = xIndex,
                YIndex = yIndex,
                Facing = itemFacing
            };

            var record = new PlacedItemRecord
            {
                Instance = placedItem,
                Data = placeData,
                Tiles = new List<PathFindableTile>()
            };

            for (var y = yIndex; y < yIndex + rotatedCellSize.y; y++)
            {
                for (var x = xIndex; x < xIndex + rotatedCellSize.x; x++)
                {
                    var tileData = Grid.GetData(x, y);
                    record.Tiles.Add(tileData);
                    placedItemRegistry[new Vector2Int(x, y)] = record;
                    tileData.SetPlaceable(false);
                    tileData.SetWalkable(item.Data.Walkable);
                    tileData.SetPenalty(Board.GetPenalty(item.Data.Category));
                }
            }

            placedItem.Place(Board);
        }


        public void PlaceTiledItem(int itemID, int xIndex, int yIndex,  ItemFacing itemFacing)
        {
            var item = tiledItemList.GetItemById(itemID);
            
            if (!item)
                return;
            
            PlaceTiledItem(item, xIndex, yIndex, itemFacing);
        }

        public void PlaceTiledItem(PlaceItemData data)
        {
            var item = tiledItemList.GetItemById(data.ItemID);
            
            if (!item)
                return;
            
            PlaceTiledItem(item, data.XIndex, data.YIndex, data.Facing);
        }

        public void RemoveTiledItem(Vector2Int cellIndex)
        {
            if (!Board)
                return;

            if (!placedItemRegistry.TryGetValue(cellIndex, out var record))
                return;

            if (record == null)
                return;

            foreach (var tile in record.Tiles.Where(tile => tile != null))
            {
                tile.ResetBuildState();
                placedItemRegistry.Remove(new Vector2Int(tile.XIndex, tile.YIndex));
            }

            if (record.Instance)
                PoolManager.Instance.Despawn(record.Instance.transform);

            EventManager.RaiseEvent(new OnRemoveItem
            {
                Data = record.Data
            });
        }

        public Vector3 GetRotatedPlaceItemPos(TiledPlaceable item, int xIndex, int yIndex, ItemFacing facing)
        {
            if(!item)
                return Vector3.zero;
            
            var rot = Quaternion.Euler(0, (int)facing, 0);
            
            var rotatedOffset = rot * item.AnchorCenterOffset;
            
            var rotatedFootprint = rot * new Vector3(item.Width, 0, item.Height);
            var correction = new Vector3(Mathf.Max(0, -rotatedFootprint.x), 0, Mathf.Max(0, -rotatedFootprint.z));
            
            return Grid.GetCellCorner(xIndex, yIndex, CornerType.LeftBottom) + rotatedOffset + correction;
        }

        public Vector3 GetRotatedPlaceItemPos(PlaceItemData data)
        {
            var item = tiledItemList.GetItemById(data.ItemID);
            return GetRotatedPlaceItemPos(item, data.XIndex, data.YIndex, data.Facing);
        }
        
        private Vector2Int WorldSizeToCellSize(float width, float height)
        {
            return new Vector2Int
            {
                x = Mathf.CeilToInt(width / Grid.CellSize),
                y = Mathf.CeilToInt(height / Grid.CellSize)
            };
        }

        private Vector2Int GetRotatedItemCellSize(Vector2Int cellSize, ItemFacing itemFacing)
        {
            return itemFacing switch
            {
                ItemFacing.Up => cellSize,
                ItemFacing.Left => new Vector2Int(cellSize.y, cellSize.x),
                ItemFacing.Down => cellSize,
                ItemFacing.Right => new Vector2Int(cellSize.y, cellSize.x),
                _ => cellSize
            };
        }


        private bool CheckTilesPlaceable(int startX, int startY, int width, int height)
        {
            for (var y = startY; y < startY + height; y++)
            {
                for (var x = startX; x < startX + width; x++)
                {
                    if (!Grid.CheckCellExist(x, y))
                        return false;

                    if (!Grid.GetData(x, y).Placeable)
                        return false;
                }
            }
        
            return true;
        }

        private bool CheckItemPlaceable(int itemId, int startX, int startY)
        {
            var item = tiledItemList.GetItemById(itemId);
            if (item == null)
                return false;
            
            var itemCellSize = WorldSizeToCellSize(item.Width, item.Height);
            return CheckTilesPlaceable(startX, startY, itemCellSize.x, itemCellSize.y);
        }
    }
}
