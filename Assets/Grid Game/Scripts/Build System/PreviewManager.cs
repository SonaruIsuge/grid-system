using SNR_PathFinding;
using UnityEngine;

namespace SNR_BuildSystem
{
    public class PreviewManager : MonoBehaviour
    {
        [SerializeField] private Transform pointIndicator;
        [SerializeField] private Transform tileIndicator;

        private bool isSetup = false;
        private Grid<PathFindableTile> grid;
        private GameInputSystem input;
        private Vector3 currentPoint;        

        private Camera MainCam => Camera.main;
        
        public void Setup(Grid<PathFindableTile> grid, GameInputSystem input)
        {
            this.grid = grid;
            this.input = input;
            isSetup = true;
        }

        
        private bool TryGetMouseInGrid(out Vector3 pos, out Vector2Int index)
        {
            index = new Vector2Int();
            pos = Vector3.zero;
        
            if (!isSetup) return false;

            var plane = new Plane(Vector3.up, 0);
            var ray = MainCam.ScreenPointToRay(input.MousePosition);

            if (!plane.Raycast(ray, out var distance))
                return false;
        
            pos = ray.GetPoint(distance);

            index = grid.GetGridIndex(pos);
        
            return grid.CheckCellExist(index.x, index.y);
        }
    }
}