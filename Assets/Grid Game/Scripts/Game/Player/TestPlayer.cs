
using System;
using SNR_BuildSystem;
using SNR_PathFinding;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Visual Elements")] 
    [SerializeField] private Transform playerPosObj;
    [SerializeField] private Transform playerTileObj;
    
    private PlayerVisual playerVisual;
    private int currentItemId;
    private Vector3 mouseInGridPos;
    private Vector2Int mouseInGridIndex;
    
    private Camera MainCam => Camera.main;
    private Grid<PathFindableTile> Grid => gameManager.GameBoard.Grid;
    private GameInputSystem Input => gameManager.GameInput;


    private void Awake()
    {
        playerVisual = new PlayerVisual(playerPosObj, playerTileObj);
        currentItemId = -1;
    }


    private void OnEnable()
    {
        gameManager.UIManager.OnItemButtonClick += SetCurrentItem;
    }


    private void OnDisable()
    {
        gameManager.UIManager.OnItemButtonClick -= SetCurrentItem;
    }


    private void Update()
    {
        if (TryGetMouseInGrid(out mouseInGridPos, out mouseInGridIndex))
        {
            playerVisual.UpdatePlayerGrid(mouseInGridPos, Grid.GetWorldPosition(mouseInGridIndex.x, mouseInGridIndex.y));
            
            if (Input.LeftMouseDown)
            {
                gameManager.GridBuildManager.PlaceTiledItem(currentItemId, mouseInGridIndex.x, mouseInGridIndex.y);
            }
        }
    }


    private bool TryGetMouseInGrid(out Vector3 pos, out Vector2Int index)
    {
        index = new Vector2Int();
        pos = Vector3.zero;
        
        var plane = new Plane(Vector3.up, 0);
        var ray = MainCam.ScreenPointToRay(Input.MousePosition);

        if (!plane.Raycast(ray, out var distance))
            return false;
        
        pos = ray.GetPoint(distance);

        index = Grid.GetGridIndex(pos);
        
        return Grid.CheckCellExist(index.x, index.y);
    }


    private void SetCurrentItem(int id)
    {
        currentItemId = id;
    }
}
