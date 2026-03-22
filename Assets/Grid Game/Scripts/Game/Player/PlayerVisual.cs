using SNR_PathFinding;
using UnityEngine;


public class PlayerVisual
{
    private Transform playerPointObj;
    private Transform playerTileObj;

    public PlayerVisual(Transform pointObj, Transform tileObj)
    {
        playerPointObj = pointObj;
        playerTileObj = tileObj;
    }


    public void UpdatePlayerGrid(Vector3 mousePos, Vector3 tilePos)
    {
        playerPointObj.position = mousePos;
        playerTileObj.position = tilePos;
    }
}
