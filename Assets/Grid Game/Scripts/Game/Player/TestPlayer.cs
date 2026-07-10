
using SNR_BuildSystem;
using SNR_Event;
using SNR_PathFinding;
using SonaruUtilities;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public enum PlayerMode
    {
        Idle,
        Placing,
        Removing,
        PlacingNpc
    }

    [SerializeField] private PlayerVisual playerVisual = new();
    [SerializeField] private BuildPreview preview = new();
    [SerializeField] private NpcPathFindSpawner npcSpawner = new();
    
    private PlaceItemData currentItemData;
    private PlayerMode currentPlayerMode;
    
    private Vector3 mouseInGridPos;
    private Vector2Int mouseInGridIndex;
    
    private Camera MainCam => Camera.main;
    private Grid<PathFindableTile> Grid => GameManager.Instance.GameBoard.Grid;
    private GameInputSystem Input => GameManager.Instance.GameInput;


    private void Awake()
    {
        currentItemData = new PlaceItemData
        {
            ItemID = -1,
            XIndex = 0,
            YIndex = 0,
            Facing = ItemFacing.Up
        };
        currentPlayerMode = PlayerMode.Idle;
    }


    private void OnEnable()
    {
        EventManager.Register<OnSelectPlaceableItem>(Event_OnSelectPlaceableItem);
        EventManager.Register<OnChangePlayerMode>(Event_OnChangePlayerMode);
    }


    private void OnDisable()
    {
        EventManager.Unregister<OnSelectPlaceableItem>(Event_OnSelectPlaceableItem);
        EventManager.Unregister<OnChangePlayerMode>(Event_OnChangePlayerMode);
    }


    private void Update()
    {
        if (!TryGetMouseInGrid(out mouseInGridPos, out mouseInGridIndex)) 
            return;
        
        currentItemData.XIndex = mouseInGridIndex.x;
        currentItemData.YIndex = mouseInGridIndex.y;
        
        playerVisual.UpdatePlayerGrid(mouseInGridPos, Grid.GetWorldPosition(currentItemData.XIndex, currentItemData.YIndex));

        switch (currentPlayerMode)
        {
            case PlayerMode.Placing:
                UpdatePlacement();
                break;
            case PlayerMode.Removing:
                UpdateRemoval();
                break;
            case PlayerMode.PlacingNpc:
                UpdateNpcPoints();
                break;
        }
    }

    private void UpdatePlacement()
    {
        if (currentItemData.ItemID < 0)
            return;
        
        var pos = GridBuildManager.Instance.GetRotatedPlaceItemPos(currentItemData);
        var rotate = Quaternion.Euler(0, (int)currentItemData.Facing, 0);
        preview.UpdatePreview(pos, rotate);
        
        if (Input.RotateObj)
        {
            RotateItem();
        }
        
        if (Input.LeftMouseDown)
        {
            PlaceItem();
        }
    }


    private void UpdateRemoval()
    {
        if (Input.LeftMouseDown)
        {
            GridBuildManager.Instance.RemoveTiledItem(mouseInGridIndex);
        }
    }

    private void UpdateNpcPoints()
    {
        if (!Input.LeftMouseDown) 
            return;
        
        if(npcSpawner.CurrentPhase == NpcPathFindSpawner.SetPhase.SetStart)
            npcSpawner.SpawnNpc(mouseInGridPos);
        else
            npcSpawner.SetTarget(mouseInGridPos);
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

    private void RotateItem()
    {
        currentItemData.Facing = currentItemData.Facing.Next();
        
        EventManager.RaiseEvent(new OnRotateItem
        {
            Facing = currentItemData.Facing
        });
    }

    private void PlaceItem()
    {
        GridBuildManager.Instance.PlaceTiledItem(currentItemData);
        
        EventManager.RaiseEvent(new OnPlaceItem
        {
            Data = currentItemData
        });
    }
    
    private void Event_OnSelectPlaceableItem(OnSelectPlaceableItem args)
    {
        if (currentPlayerMode == PlayerMode.Removing)
            return;
        
        var itemData = GridBuildManager.Instance.GetTiledItemData(args.Id);

        if (itemData == null)
            return;
        
        currentItemData.ItemID = args.Id;
        currentItemData.Facing = ItemFacing.Up;
        currentPlayerMode = PlayerMode.Placing;
        preview.SetPreview(itemData.PreviewObj);
    }

    private void Event_OnChangePlayerMode(OnChangePlayerMode args)
    {
        currentItemData.ItemID = -1;
        preview.ClearPreview();

        currentPlayerMode = args.Mode;
    }
}
