
using UnityEngine;

namespace SNR_BuildSystem
{
    public class TiledPlaceable : MonoBehaviour, IPlaceable
    {
        public Transform Anchor => GetAnchor();
        [field: SerializeField] public PlaceableData Data { get; private set; }
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