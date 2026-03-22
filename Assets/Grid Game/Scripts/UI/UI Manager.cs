using System;
using SNR_BuildSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Build Button")]
    [SerializeField] private Button[] itemButtons;

    public event Action<int> OnItemButtonClick;

    
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
            itemButtons[i].onClick.AddListener(() => OnItemButtonClick?.Invoke(tiledItemList.Items[itemIndex].Data.ID));
        }
    }


    public void UnregisterItemButtons()
    {
        foreach (var button in itemButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
