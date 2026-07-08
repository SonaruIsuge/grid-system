
using SNR_BuildSystem;
using SNR_Event;
using UnityEngine;
using UtilSNR.Pool;

public class UIManager : MonoBehaviour
{
    [Header("Build Button")]
    [SerializeField] private Transform btnParent;
    [SerializeField] private BuildingItemButton buildingBtnPrefab;
    
    public void RegisterItemButtons(TiledItemList tiledItemList)
    {
        for (var i = 0; i < tiledItemList.Items.Count; i++)
        {
            if (tiledItemList.Items.Count < i)
            {
                return;
            }

            var itemBtn = PoolManager.Instance.Spawn(buildingBtnPrefab, btnParent);
            itemBtn.SetData(tiledItemList.Items[i]);
        }
    }


    public void UnregisterItemButtons()
    {
        
    }

    private void OnButtonClick(int id)
    {
        EventManager.RaiseEvent(new OnSelectPlaceableItem
        {
            Id = id
        });
    }
}
