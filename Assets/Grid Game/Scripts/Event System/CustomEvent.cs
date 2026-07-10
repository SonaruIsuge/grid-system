using SNR_BuildSystem;
using SNR_PathFinding;
using UnityEngine;

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

    public struct OnRotateItem : CustomEvent
    {
        public ItemFacing Facing;
    }

    public struct OnPlaceItem : CustomEvent
    {
        public PlaceItemData Data;
    }

    public struct OnRemoveItem : CustomEvent
    {
        public PlaceItemData Data;
    }

    public struct OnChangePlayerMode : CustomEvent
    {
        public TestPlayer.PlayerMode Mode;
    }

    public struct OnNpcFindPath : CustomEvent
    {
        public bool HasPath;
        public TestNPC Npc;
        public Vector3[] WayPoints;
        public Path Path;
    }

    public struct OnSpawnNpc : CustomEvent
    {
        public TestNPC Npc;
    }
    
    public struct OnSetNpcTarget : CustomEvent
    {
        public TestNPC Npc;
        public Vector3 TargetPos;
    }

    public struct OnNpcReachTarget : CustomEvent
    {
        public TestNPC Npc;
    }
}
