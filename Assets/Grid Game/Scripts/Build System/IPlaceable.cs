
using UnityEngine;

namespace SNR_BuildSystem
{
    public interface IPlaceable
    {
        Transform Anchor { get; }
        PlaceableData Data { get; }
        bool Placed { get; }


        void Place(GameBoard gameBoard);
    }
}