using SNR_Event;
using SonaruUtilities;
using UnityEngine;

namespace SNR_PathFinding
{
    public class PathFindableTile : IGridTile, IHeapItem<PathFindableTile>
    {
        private Grid<PathFindableTile> grid;
        
        public int XIndex { get; private set; }
        public int YIndex { get; private set; }
        public Vector3 WorldPos => grid?.GetWorldPosition(XIndex, YIndex) ?? Vector3.zero;
        
        public bool Walkable { get; private set; }
        public int HeapIndex { get; set; }
        public int PathPenalty { get; private set; }
        public bool Placeable { get; private set; }
        
        public int GCost;
        public int HCost;
        public int FCost;
        public PathFindableTile CameFromNode;


        public PathFindableTile(Grid<PathFindableTile> grid, int xIndex, int yIndex)
        {
            this.grid = grid;
            this.XIndex = xIndex;
            this.YIndex = yIndex;
            Walkable = true;
            PathPenalty = 0;
            Placeable = true;
            
            GCost = 0;
            HCost = 0;
            FCost = 0;
            CameFromNode = null;
        }


        public void SetWalkable(bool walkable)
        {
            if(Walkable == walkable)
                return;
            
            Walkable = walkable;
            EventManager.RaiseEvent(new OnTileChangeWalkable(this, walkable));
        }


        public void SetPenalty(int penalty)
        {
            PathPenalty = penalty;
        }


        public void SetPlaceable(bool placeable)
        {
            Placeable = placeable;
        }
        

        public void CalculateFCost()
        {
            FCost = GCost + HCost;
        }

        
        public void ResetPathData()
        {
            GCost = int.MaxValue;
            HCost = 0;
            CalculateFCost();
            CameFromNode = null;
        }

        
        // return 1 : the f cost (or h cost) is less than other
        // return 0 : the f cost and h cost of the two tile are the same
        // return -1 : the f cost ( or h cost) is greater than other
        public int CompareTo(PathFindableTile other)
        {
            var compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);
            return -compare;
        }
    }
}