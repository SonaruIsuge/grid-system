using SNR_BuildSystem;
using SNR_Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BuildingItemButton : Button
{
    [SerializeField] private TMP_Text buttonName;

    private TiledPlaceable tileData;
    
    protected override void OnDisable()
    {
        onClick.RemoveAllListeners();
    }

    public void SetData(TiledPlaceable tile)
    {
        tileData = tile;
        buttonName.text = tileData.Data.Name;
        onClick.AddListener(Event_OnButtonClick);
    }

    private void Event_OnButtonClick()
    {
        if (!tileData)
            return;

        EventManager.RaiseEvent(new OnSelectPlaceableItem
        {
            Id = tileData.Data.ID
        });
    }

}
