
using SNR_BuildSystem;
using SNR_Event;
using UnityEngine;
using UtilSNR.Pool;

public class UIManager : MonoBehaviour
{
    [Header("Build Button")]
    [SerializeField] private Transform btnParent;
    [SerializeField] private BuildingItemButton buildingBtnPrefab;
    
    [Header("Remove Button")]
    [SerializeField] private PlayerToolButton[] playerToolButtons;
    
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
        
        EventManager.Register<OnChangePlayerMode>(Event_OnChangePlayerMode);
    }


    public void UnregisterItemButtons()
    {
        EventManager.Unregister<OnChangePlayerMode>(Event_OnChangePlayerMode);
    }

    private void Event_OnChangePlayerMode(OnChangePlayerMode args)
    {
        foreach (var item in playerToolButtons)
        {
            item.EnableButton(args.Mode != item.Mode);
        }
    }
}
