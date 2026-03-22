using System;
using System.Collections.Generic;
using SNR_PathFinding;
using Unity.Mathematics;
using UnityEngine;


public class GameBoard : MonoBehaviour
{
    [SerializeField] private int rowNumber;
    [SerializeField] private int columnNumber;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 offsetPosition;

    [SerializeField] private TilePenaltySetting penaltySetting;

    [Header("Path Finding Preference")]
    [SerializeField] private bool blurPenalty;
    [SerializeField] [Range(0, 10)] private int blurSize = 3;

    private Grid<PathFindableTile> grid;
    private Dictionary<TileCategory, int> penaltyDict;
    
    public Grid<PathFindableTile> Grid => grid;

    // for visual debug
    [HideInInspector] public int maxPenalty = int.MinValue;
    [HideInInspector] public int minPenalty = int.MaxValue;
    public int RowNumber => rowNumber;
    public int ColumnNumber => columnNumber;
    public float CellSize => cellSize;
    public Vector3 OffsetPosition => offsetPosition;
    
    private void Awake()
    {
        penaltyDict = penaltySetting.GenerateDictionary();
        grid = new Grid<PathFindableTile>(rowNumber, columnNumber, cellSize, offsetPosition, CreateGridTile);
    }


    private void Start()
    {
        AutoSetObstacle();
        if(blurPenalty) BlurPenaltyMap();
    }


    public void InitBoardValue(float size, int w, int h, Vector3 offsetPos, TilePenaltySetting setting)
    {
        cellSize = size;
        columnNumber = w;
        rowNumber = h;
        offsetPosition = offsetPos;
        penaltySetting = setting;
    }


    public int GetPenalty(TileCategory category)
    {
        if (penaltyDict == null || !penaltyDict.ContainsKey(category))
            return 0;
        
        return penaltyDict[category];
    }


    private PathFindableTile CreateGridTile(Grid<PathFindableTile> grid, int xIndex, int yIndex)
    {
        return new PathFindableTile(grid, xIndex, yIndex);
    }
    
    
    private void AutoSetObstacle()
    {
        var results = new RaycastHit[3];
        for (var x = 0; x < grid.ColumnNumber; x++)
        {
            for (var y = 0; y < grid.RowNumber; y++)
            {
                var tile = grid.GetData(x, y);
                var ray = new Ray(tile.WorldPos + Vector3.up * 5, Vector3.down);
                
                Physics.RaycastNonAlloc(ray, results, 10, penaltySetting.GridLayer);
                
                var penalty = int.MaxValue;
                foreach (var result in results)
                {
                    if(!result.collider || !result.collider.TryGetComponent<TileCategoryMarker>(out var marker))
                        continue;
                    
                    // If any tile is marked as obstacle, set as obstacle
                    if (marker.TileCategory == TileCategory.Obstacle)
                    {
                        tile.SetWalkable(false);
                        penalty = penaltyDict[TileCategory.Obstacle];
                        break;
                    }
                    
                    // If there are two categories be detected, use the one with small penalty
                    if (penaltyDict.ContainsKey(marker.TileCategory))
                    {
                        if (penaltyDict[marker.TileCategory] < penalty)
                            penalty = penaltyDict[marker.TileCategory];
                    }
                }
                
                tile.SetPenalty(penalty);
                
                
                results = new RaycastHit[3];
            }
        }
    }


    private void BlurPenaltyMap()
    {
        var kernelSize = blurSize * 2 + 1;
        var kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltyHorizontalPass = new int[columnNumber, rowNumber];
        int[,] penaltyVerticalPass = new int[columnNumber, rowNumber];

        for (var y = 0; y < rowNumber; y++)
        {
            for (var x = -kernelExtents; x <= kernelExtents; x++)
            {
                var sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltyHorizontalPass[0, y] += grid.GetData(sampleX, y).PathPenalty;
            }

            for (var x = 1; x < columnNumber; x++)
            {
                var removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, columnNumber);
                var addIndex = Mathf.Clamp(x + kernelExtents, 0, columnNumber - 1);
                
                penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] -
                    grid.GetData(removeIndex, y).PathPenalty + grid.GetData(addIndex, y).PathPenalty;
            }
        }

        for (var x = 0; x < columnNumber; x++)
        {
            for (var y = -kernelExtents; y <= kernelExtents; y++)
            {
                var sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltyVerticalPass[x, 0] += penaltyHorizontalPass[x, sampleY];
            }
            
            var blurPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid.GetData(x, 0).SetPenalty(blurPenalty);

            for (var y = 1; y < rowNumber; y++)
            {
                var removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, rowNumber);
                var addIndex = Mathf.Clamp(y + kernelExtents, 0, rowNumber - 1);

                penaltyVerticalPass[x, y] = penaltyVerticalPass[x, y - 1] - 
                                            penaltyHorizontalPass[x, removeIndex] + penaltyHorizontalPass[x, addIndex];
                blurPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, y] / (kernelSize * kernelSize));
                
                grid.GetData(x, y).SetPenalty(blurPenalty);

                maxPenalty = Mathf.Max(blurPenalty, maxPenalty);
                minPenalty = Mathf.Min(blurPenalty, minPenalty);
            }
        }
    }
}
