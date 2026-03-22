using System;
using System.Collections.Generic;
using UnityEngine;
using SonaruUtilities;

namespace SNR_PathFinding
{
    public class PathFinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private Grid<PathFindableTile> currentGrid;
        private Heap<PathFindableTile> openList;
        private HashSet<PathFindableTile> closeList;

        
        public PathFinding()
        {
            currentGrid = null;
            closeList = new HashSet<PathFindableTile>();
        }
        
        /// <summary>
        /// Finding the path from the start tile to the end tile.
        /// </summary>
        /// <returns>Every tile in the path; if the path is inaccessible, return null</returns>
        public List<PathFindableTile> FindPath(Grid<PathFindableTile> grid, PathFindableTile startTile, PathFindableTile endTile)
        {
            UpdateCurrentGrid(grid);
            var pathFind = false;
            
            if (!startTile.Walkable || !endTile.Walkable)
                return null;
            
            //var sw = new Stopwatch();
            //sw.Start();
            
            openList.Clear();
            closeList.Clear();
            openList.Add(startTile);

            for (var x = 0; x < grid.ColumnNumber; x++)
            {
                for (var y= 0; y < grid.RowNumber; y++)
                {
                    grid.GetData(x, y).ResetPathData();
                }
            }
            
            startTile.GCost = 0;
            startTile.HCost = CalculateDistanceCost(startTile, endTile);
            startTile.CalculateFCost();

            while (openList.Count > 0)
            {
                var currentTile = openList.RemoveFirst();
                closeList.Add(currentTile);

                if (currentTile == endTile)
                {
                    //sw.Stop();
                    //Log.Info($"path find: {sw.ElapsedMilliseconds} ms");
                    
                    pathFind = true;
                    break;
                }
                
                foreach (var neighborTile in GetNeighborList(currentTile))
                {
                    if(closeList.Contains(neighborTile))
                        continue;
                    
                    if(!neighborTile.Walkable)
                        continue;

                    var tentativeGCost = currentTile.GCost + CalculateDistanceCost(currentTile, neighborTile) + neighborTile.PathPenalty;
                    if (tentativeGCost < neighborTile.GCost)
                    {
                        neighborTile.CameFromNode = currentTile;
                        neighborTile.GCost = tentativeGCost;
                        neighborTile.HCost = CalculateDistanceCost(neighborTile, endTile);
                        neighborTile.CalculateFCost();

                        if (!openList.Contains(neighborTile))
                        {
                            openList.Add(neighborTile);
                        }
                        else
                        {
                            openList.UpdateItem(neighborTile);
                        }
                    }
                }
            }
            return pathFind ? CalculatePath(endTile) : null;
        }


        /// <summary>
        /// Finding the simplified path (only record points of each corner) from the start tile to the end tile.
        /// </summary>
        /// <param name="request">Path finding request. (Includes the callback after operation.)</param>
        /// <param name="callback">If the calculation is finished, callback will be called.</param>
        public void FindSimplifyPath(PathRequest request, Action<PathResult> callback)
        {
            var startTile = request.Grid.GetData(request.StartPos);
            var endTile = request.Grid.GetData(request.EndPos);
            var path = FindPath(request.Grid, startTile, endTile);

            if (path == null)
            {
                callback(new PathResult(null, false, request.Callback));
                return;
            }

            var wayPoints = new List<Vector3>();
            var oldVec2Dir = Vector2.zero;
        
            //wayPoints.Add(startTile.WorldPos);
            for (var i = 1; i < path.Count; i++)
            {
                var newDirection = path[i].WorldPos - path[i - 1].WorldPos;
                var newVec2Dir = new Vector2(newDirection.x, newDirection.z);
                
                if(oldVec2Dir == newVec2Dir)
                    continue;
                
                wayPoints.Add(path[i-1].WorldPos);
                oldVec2Dir = newVec2Dir;
            }
            wayPoints.Add(endTile.WorldPos);

            callback(new PathResult(wayPoints.ToArray(), true, request.Callback));
        }


        private void UpdateCurrentGrid(Grid<PathFindableTile> newGrid)
        {
            if (newGrid == currentGrid) return;
            
            currentGrid = newGrid;
            openList?.Clear();
            openList = new Heap<PathFindableTile>(currentGrid.MaxCellNumber);
        }


        private List<PathFindableTile> CalculatePath(PathFindableTile endTile)
        {
            var path = new List<PathFindableTile> { endTile };
            var currentTile = endTile;
            while(currentTile.CameFromNode != null)
            {
                path.Add(currentTile.CameFromNode);
                currentTile = currentTile.CameFromNode;
            }
            path.Reverse();
            return path;
        }
        
        
        private int CalculateDistanceCost(IGridTile a, IGridTile b)
        {
            var xDis = Mathf.Abs(a.XIndex - b.XIndex);
            var yDis = Mathf.Abs(a.YIndex - b.YIndex);
            var remaining = Mathf.Abs(xDis - yDis);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDis, yDis) + MOVE_STRAIGHT_COST * remaining;
        }


        private List<PathFindableTile> GetNeighborList(PathFindableTile currentTile)
        {
            var neighborList = new List<PathFindableTile>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0)
                        continue;

                    var xIndex = currentTile.XIndex + x;
                    var yIndex = currentTile.YIndex + y;
                    
                    if(!currentGrid.CheckCellExist(xIndex, yIndex))
                        continue;
                    
                    neighborList.Add(currentGrid.GetData(xIndex, yIndex));
                }
            }
            return neighborList;
        }
    }
}