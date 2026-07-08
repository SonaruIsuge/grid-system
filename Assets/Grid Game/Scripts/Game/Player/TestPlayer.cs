
using SNR_BuildSystem;
using SNR_Event;
using SNR_PathFinding;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] 
    private PlayerVisual playerVisual = new();

    [SerializeField] 
    private BuildPreview preview = new();
    
    // private PlayerVisual playerVisual;
    // private BuildPreview preview;
    private int currentItemId;
    private Vector3 mouseInGridPos;
    private Vector2Int mouseInGridIndex;
    
    private Camera MainCam => Camera.main;
    private Grid<PathFindableTile> Grid => GameManager.Instance.GameBoard.Grid;
    private GameInputSystem Input => GameManager.Instance.GameInput;


    private void Awake()
    {
        // playerVisual = new PlayerVisual(playerPosObj, playerTileObj);
        // preview = new BuildPreview();
        currentItemId = -1;
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

        var tilePos = Grid.GetWorldPosition(mouseInGridIndex.x, mouseInGridIndex.y);
        
        playerVisual.UpdatePlayerGrid(mouseInGridPos, tilePos);
        preview.UpdatePreview(mouseInGridIndex.x, mouseInGridIndex.y);
        
        // Place the item
        if (Input.LeftMouseDown)
        {
            GridBuildManager.Instance.PlaceTiledItem(currentItemId, mouseInGridIndex.x, mouseInGridIndex.y);
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

    private void Event_OnSelectPlaceableItem(OnSelectPlaceableItem args)
    {
        var itemData = GridBuildManager.Instance.GetTiledItemData(args.Id);

        if (itemData == null)
            return;
        
        currentItemId = args.Id;
        preview.SetPreview(itemData.PreviewObj);
    }
}
