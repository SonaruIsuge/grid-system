using System;
using SNR_BuildSystem;
using SNR_Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Build Button")]
    [SerializeField] private Button[] itemButtons;
    
    public void RegisterItemButtons(TiledItemList tiledItemList)
    {
        for (var i = 0; i < itemButtons.Length; i++)
        {
            if (tiledItemList.Items.Count < i)
            {
                return;
            }

            var itemIndex = i;
            itemButtons[i].GetComponentInChildren<TMP_Text>().text = tiledItemList.Items[i].Data.Name;
            itemButtons[i].onClick.AddListener(() => OnButtonClick(tiledItemList.Items[itemIndex].Data.ID));
        }
    }


    public void UnregisterItemButtons()
    {
        foreach (var button in itemButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void OnButtonClick(int id)
    {
        EventManager.RaiseEvent(new OnSelectPlaceableItem
        {
            Id = id
        });
    }
}
