using System.Collections.Generic;
using UnityEngine;

namespace SonaruUtilities
{
    public class KeyValueScriptableObject<TKey, TValue> : ScriptableObject 
    {
        [SerializeField] private List<TKey> keys;
        [SerializeField] private List<TValue> values;
        
            
        public Dictionary<TKey, TValue> GenerateDictionary()
        {
            var result = new Dictionary<TKey, TValue>();
            
            for (var i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }
        
        
    }
}