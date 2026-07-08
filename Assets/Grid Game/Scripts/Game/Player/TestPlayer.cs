
using SNR_BuildSystem;
using SNR_Event;
using SNR_PathFinding;
using SonaruUtilities;
using Unity.Mathematics;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerVisual playerVisual = new();
    [SerializeField] private BuildPreview preview = new();
    
    private PlaceItemData currentItemData;
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
    }


    private void OnEnable()
    {
        EventManager.Register<OnSelectPlaceableItem>(Event_OnSelectPlaceableItem);
    }


    private void OnDisable()
    {
        EventManager.Unregister<OnSelectPlaceableItem>(Event_OnSelectPlaceableItem);
    }


    private void Update()
    {
        if (!TryGetMouseInGrid(out mouseInGridPos, out mouseInGridIndex)) 
            return;
        
        currentItemData.XIndex = mouseInGridIndex.x;
        currentItemData.YIndex = mouseInGridIndex.y;
        
        playerVisual.UpdatePlayerGrid(mouseInGridPos, Grid.GetWorldPosition(currentItemData.XIndex, currentItemData.YIndex));

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
        var itemData = GridBuildManager.Instance.GetTiledItemData(args.Id);

        if (itemData == null)
            return;
        
        currentItemData.ItemID = args.Id;
        currentItemData.Facing = ItemFacing.Up;
        preview.SetPreview(itemData.PreviewObj);
    }
}
