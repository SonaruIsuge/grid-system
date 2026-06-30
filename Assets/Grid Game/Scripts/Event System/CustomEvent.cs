using SNR_PathFinding;

namespace SNR_Event
{
    public interface CustomEvent { }


    public struct OnGridDataChanged<TGridData> : CustomEvent where TGridData : IGridTile
    {
        public Grid<TGridData> Grid;
        public int XIndex;
        public int YIndex;
        public TGridData Data;
    }


    public struct OnTileChangeWalkable : CustomEvent
    {
        public IGridTile Tile;
        public bool Walkable;
    }


    public struct OnTileChangePenalty : CustomEvent
    {
        public PathFindableTile Tile;
        public TileCategory Category;
    }

    public struct OnSelectPlaceableItem : CustomEvent
    {
        public int Id;
    }
}