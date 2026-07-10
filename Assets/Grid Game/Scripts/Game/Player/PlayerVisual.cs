using SNR_PathFinding;
using UnityEngine;


[System.Serializable]
public class PlayerVisual
{
    [SerializeField] private Transform playerPointObj;
    [SerializeField] private Transform playerTileObj;


    public void UpdatePlayerGrid(Vector3 mousePos, Vector3 tilePos)
    {
        if(playerPointObj) playerPointObj.position = mousePos;
        if(playerTileObj) playerTileObj.position = tilePos;
    }
}
