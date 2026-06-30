
using UnityEngine;

namespace SNR_BuildSystem
{
    public class TiledPlaceable : MonoBehaviour, IPlaceable
    {
        [SerializeField] private Transform anchor;
        
        [SerializeField] private PlaceableData placeableData;
        
        public Transform Anchor => anchor;
        public PlaceableData Data => placeableData;
        public bool Placed { get; private set; }

        public float Width => Data.WorldSize.x;
        public float Height => Data.WorldSize.y;
        public Vector3 AnchorCenterOffset => transform.position - Anchor.position;
        
        
        public void Place(GameBoard gameBoard)
        {
            Placed = true;
        }


        private Transform GetAnchor()
        {
            var transforms = GetComponentsInChildren<Transform>();
            return transforms.Length > 1 ? transforms[1] : transforms[0];
        }
    }
}