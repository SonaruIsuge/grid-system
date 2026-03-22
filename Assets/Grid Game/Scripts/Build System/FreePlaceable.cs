
using UnityEngine;

namespace SNR_BuildSystem
{
    public class FreePlaceable : MonoBehaviour, IPlaceable
    {
        public Transform Anchor { get; private set; }
        public PlaceableData Data { get; private set; }
        public bool Placed { get; private set; }
        
        
        public void Place(GameBoard gameBoard)
        {
            Placed = true;
        }
    }

}