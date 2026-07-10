
using SNR_Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class PlayerToolButton : MonoBehaviour
{
    [SerializeField] private TestPlayer.PlayerMode mode;
    
    private Button button;
    
    public TestPlayer.PlayerMode Mode => mode;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(RemoveModeButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(RemoveModeButtonClick);
    }

    public void EnableButton(bool enable)
    {
        button.interactable = enable;
    }

    private void RemoveModeButtonClick()
    {
        EventManager.RaiseEvent(new OnChangePlayerMode
        {
            Mode = mode
        });
    }
}
