using SNR_PathFinding;

namespace SNR_Event
{
    public abstract class CustomEvent { }


    public class OnGridDataChanged<TGridData> : CustomEvent where TGridData : IGridTile
    {
        public Grid<TGridData> grid;
        public int xIndex;
        public int yIndex;
        public TGridData data;

        public OnGridDataChanged(Grid<TGridData> grid, int xIndex, int yIndex, TGridData data)
        {
            this.grid = grid;
            this.xIndex = xIndex;
            this.yIndex = yIndex;
            this.data = data;
        }
    }


    public class OnTileChangeWalkable : CustomEvent
    {
        public IGridTile Tile;
        public bool Walkable;

        public OnTileChangeWalkable(IGridTile tile, bool walkable)
        {
            this.Tile = tile;
            this.Walkable = walkable;
        }
    }


    public class OnTileChangePenalty : CustomEvent
    {
        public PathFindableTile Tile;
        public TileCategory Category;

        public OnTileChangePenalty(PathFindableTile tile, TileCategory category)
        {
            this.Tile = tile;
            this.Category = category;
        }
    }
}