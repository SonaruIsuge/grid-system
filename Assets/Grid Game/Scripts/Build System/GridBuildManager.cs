
using SNR_PathFinding;
using UnityEngine;
using UtilSNR.Common;
using UtilSNR.Pool;

namespace SNR_BuildSystem
{
    public enum ItemFacing
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public class GridBuildManager : TSceneSingletonBehaviour<GridBuildManager>
    {
        [SerializeField] private TiledItemList tiledItemList;

        public TiledItemList TiledItemList => tiledItemList;
        
        private GameBoard Board => GameManager.Instance != null ? GameManager.Instance.GameBoard : null;
        private Grid<PathFindableTile> Grid => Board != null? Board.Grid : null;


        public PlaceableData GetTiledItemData(int id)
        {
            var item = tiledItemList.GetItemById(id);
            
            return item ? item.Data : null;
        }
        

        public void PlaceTiledItem(TiledPlaceable item, int xIndex, int yIndex)
        {
            if (!Board)
                return;

            var itemCellSize = WorldSizeToCellSize(item.Width, item.Height);
            if(!CheckTilesPlaceable(xIndex, yIndex, itemCellSize.x, itemCellSize.y))
                return;
            
            var initItemPos = Grid.GetCellCorner(xIndex, yIndex, CornerType.LeftBottom) + item.AnchorCenterOffset;
            PoolManager.Instance.Spawn(item, initItemPos, Quaternion.identity);
            
            for (var y = yIndex; y < yIndex + itemCellSize.y; y++)
            {
                for (var x = xIndex; x < xIndex + itemCellSize.x; x++)
                {
                    var tileData = Grid.GetData(x, y);
                    tileData.SetPlaceable(false);
                    tileData.SetWalkable(item.Data.Walkable);
                    tileData.SetPenalty(Board.GetPenalty(item.Data.Category));
                }
            }

            item.Place(Board);
        }


        public void PlaceTiledItem(int itemID, int xIndex, int yIndex)
        {
            var item = tiledItemList.GetItemById(itemID);
            
            if (!item)
                return;
            
            PlaceTiledItem(item, xIndex, yIndex);
        }


        private Vector2Int WorldSizeToCellSize(float width, float height)
        {
            return new Vector2Int
            {
                x = Mathf.CeilToInt(width / Grid.CellSize),
                y = Mathf.CeilToInt(height / Grid.CellSize)
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
