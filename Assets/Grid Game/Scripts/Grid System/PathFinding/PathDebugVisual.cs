
using System;
using System.Collections.Generic;
using System.Linq;
using SNR_BuildSystem;
using SNR_Event;
using UnityEngine;

namespace SNR_PathFinding
{
    
    public class PathDebugVisual : MonoBehaviour
    {
        // Reference
        [SerializeField] private GameBoard gameBoard;
        
        // Debug Visual Control
        [SerializeField] private bool drawTile;
        [SerializeField] private bool drawBoard;
        [SerializeField] private bool drawObstacle;
        [SerializeField] private bool drawPath;
        
        private Grid<PathFindableTile> Grid => gameBoard.Grid;
        private Vector3 leftTop;
        private Vector3 rightTop;
        private Vector3 leftBottom;
        private Vector3 rightBottom;
        private List<Vector3> obstaclesList;
        private Dictionary<TestNPC, Vector3[]> npcWayPointsDict;
        private Dictionary<TestNPC, Path> npcPathDict;


        public void SetGameBoard(GameBoard board)
        {
            gameBoard = board;
        }
        

        private void Awake()
        {
            obstaclesList = new List<Vector3>();

            npcWayPointsDict = new Dictionary<TestNPC, Vector3[]>();
            npcPathDict = new Dictionary<TestNPC, Path>();
        }


        private void OnEnable()
        {
            EventManager.Register<OnTileChangeWalkable>(RecordObstacle);
            EventManager.Register<OnNpcFindPath>(Event_OnNpcFindPath);
        }


        private void OnDisable()
        {
            EventManager.Unregister<OnTileChangeWalkable>(RecordObstacle);
            EventManager.Unregister<OnNpcFindPath>(Event_OnNpcFindPath);
        }

        
        private void Start()
        {
            leftTop = Grid.GetCellCorner(0, Grid.RowNumber - 1, CornerType.LeftTop);
            rightTop = Grid.GetCellCorner(Grid.ColumnNumber - 1, Grid.RowNumber - 1, CornerType.RightTop);
            leftBottom = Grid.GetCellCorner(0, 0, CornerType.LeftBottom);
            rightBottom = Grid.GetCellCorner(Grid.ColumnNumber - 1, 0, CornerType.RightBottom);
        }


        private void OnValidate()
        {
            var halfCellSize = new Vector3(gameBoard.CellSize / 2, 0, gameBoard.CellSize / 2);
            leftBottom = gameBoard.OffsetPosition - halfCellSize;
            leftTop = leftBottom + (Vector3.forward * gameBoard.CellSize * gameBoard.RowNumber);
            rightBottom = leftBottom + (Vector3.right * gameBoard.CellSize * gameBoard.ColumnNumber);
            rightTop = rightBottom + (Vector3.forward * gameBoard.CellSize * gameBoard.RowNumber);
        }


        private void OnDrawGizmosSelected()
        {
            if(!gameBoard)
                return;

            if (drawBoard)
                DrawGameBoard();

            if (drawTile)
                DrawTiles();
            
            if(drawObstacle)
                DrawObstacles();

            if (drawPath)
                DrawPath();
        }


        private void DrawGameBoard()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(leftTop, rightTop);
            Gizmos.DrawLine(rightTop, rightBottom);
            Gizmos.DrawLine(rightBottom, leftBottom);
            Gizmos.DrawLine(leftBottom, leftTop);
        }


        private void DrawTiles()
        {
            Gizmos.color = Color.cyan;

            if (!Application.isPlaying)
            {
                for (var i = 1; i < gameBoard.ColumnNumber; i++)
                {
                    var start = leftTop + Vector3.right * gameBoard.CellSize * i;
                    var end = leftBottom + Vector3.right * gameBoard.CellSize * i;
                    Gizmos.DrawLine(start, end);
                }

                for (var j = 1; j < gameBoard.RowNumber; j++)
                {
                    var start = leftBottom + Vector3.forward * gameBoard.CellSize * j;
                    var end = rightBottom + Vector3.forward * gameBoard.CellSize * j;
                    Gizmos.DrawLine(start, end);
                }
            }

            else
            {
                for (var i = 1; i < Grid.ColumnNumber; i++)
                {
                    var start = leftTop + Vector3.right * Grid.CellSize * i;
                    var end = leftBottom + Vector3.right * Grid.CellSize * i;
                    Gizmos.DrawLine(start, end);
                }

                for (var j = 1; j < Grid.RowNumber; j++)
                {
                    var start = leftBottom + Vector3.forward * Grid.CellSize * j;
                    var end = rightBottom + Vector3.forward * Grid.CellSize * j;
                    Gizmos.DrawLine(start, end);
                }
            }
        }


        private void DrawTileCubes()
        {
            if (!Application.isPlaying)
            {
                for (var x = 0; x < gameBoard.ColumnNumber; x++)
                {
                    for (var y = 0; y < gameBoard.RowNumber; y++)
                    {
                        Gizmos.DrawCube(new Vector3(x, 0, y) * gameBoard.CellSize + gameBoard.OffsetPosition,
                            Vector3.one * gameBoard.CellSize * 0.95f);
                    }
                }
            }

            else
            {
                for (var x = 0; x < Grid.ColumnNumber; x++)
                {
                    for (var y = 0; y < Grid.RowNumber; y++)
                    {
                        Gizmos.color = Grid.GetData(x, y).IsBuildLayerPlaceable(BuildLayer.Wall) ? Color.white : Color.black;
                        Gizmos.DrawCube(Grid.GetWorldPosition(x, y), Vector3.one * Grid.CellSize * 0.95f);
                    }
                }
            }
        }


        private void DrawObstacles()
        {
            if (!Application.isPlaying)
                return;
            
            Gizmos.color = Color.black;

            foreach (var pos in obstaclesList)
            {
                Gizmos.DrawCube(pos, Vector3.one * Grid.CellSize);
            }

        }


        private void DrawPath()
        {
            if (!Application.isPlaying)
                return;
            
            Gizmos.color = Color.green;
            
            foreach (var path in npcWayPointsDict.Values)
            {
                if(path == null || path.Length == 0)
                    continue;
                
                for (var i = 0; i < path.Length - 1; i++)
                {
                    Gizmos.DrawLine(path[i], path[i+1]);
                }
            }

            foreach (var path in npcPathDict.Values)
            {
                path?.DrawWithGizmos();
            }
        }

        private void RecordObstacle(OnTileChangeWalkable e)
        {
            var position = e.Tile.WorldPos;
            
            if(!e.Walkable && !obstaclesList.Contains(position))
                obstaclesList.Add(position);

            if (e.Walkable && obstaclesList.Contains(position))
                obstaclesList.Remove(position);
        }

        private void Event_OnNpcFindPath(OnNpcFindPath args)
        {
            if (args.HasPath)
            {
                npcWayPointsDict[args.Npc] = args.WayPoints;
                npcPathDict[args.Npc] = args.Path;
            }
            else
            {
                npcWayPointsDict[args.Npc] = null;
                npcPathDict[args.Npc] = null;
            }
        }
    }
}