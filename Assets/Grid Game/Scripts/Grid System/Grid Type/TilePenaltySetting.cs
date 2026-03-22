using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;


[CreateAssetMenu(fileName = "Tile Penalty Setting", menuName = "Grid System/Tile Penalty Setting")]

// Key: Tile category (enum)
// Value: Penalty for path finding (int)
public class TilePenaltySetting : KeyValueScriptableObject<TileCategory, int>
{
    public LayerMask GridLayer;
}
