using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace SNR_BuildSystem
{
    [CreateAssetMenu(fileName = "Tiled Item List", menuName = "Build System/Tiled Item List")]
    public class TiledItemList : ScriptableObject
    {
        public List<TiledPlaceable> Items;


        public TiledPlaceable GetItemById(int id)
        {
            foreach (var item in Items)
            {
                if (item.Data.ID == id)
                {
                    return item;
                }
            }

            return null;
        }
        
        
        [Button("Update All Items")]
        private void UpdateAllItems()
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new []{"Assets/Grid Game/Prefab"});
            
            if (prefabGuids == null)
            {
                return;
            }
            
            foreach (var prefabGuid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var tiledItem = AssetDatabase.LoadAssetAtPath<TiledPlaceable>(path);

                if (tiledItem)
                {
                    Items.Add(tiledItem);
                }
            }
        }
    }
}