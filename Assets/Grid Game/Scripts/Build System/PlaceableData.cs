using UnityEngine;

namespace SNR_BuildSystem
{
    [CreateAssetMenu(fileName = "New Placeable", menuName = "Build System/Placeable Data")]
    public class PlaceableData : ScriptableObject
    {
        public int ID;
        public string Name;
        public Vector2 WorldSize;
        public bool Walkable;
        public TileCategory Category;
        public BuildLayer BuildLayer;
        public GameObject PreviewObj;
    }
}
