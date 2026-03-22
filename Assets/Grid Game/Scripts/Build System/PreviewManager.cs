using SNR_PathFinding;
using UnityEngine;

namespace SNR_BuildSystem
{
    public class PreviewManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform pointIndicator;
        [SerializeField] private Transform tileIndicator;

        private Camera MainCam => Camera.main;
        private Grid<PathFindableTile> Grid => gameManager.GameBoard.Grid;
        private GameInputSystem Input => gameManager.GameInputSystem;

        private Vector3 currentPoint;
        
        
        
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
    }
}