using System;
using SNR_BuildSystem;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GridBuildManager GridBuildManager;
    public PreviewManager PreviewManager;
    public UIManager UIManager;
    public GameInputSystem GameInputSystem;
    public GameBoard GameBoard;


    private void Awake()
    {
        GameInputSystem.Init();
    }


    private void OnEnable()
    {
        GameInputSystem.RegisterInput();
        UIManager.RegisterItemButtons(GridBuildManager.TiledItemList);
    }


    private void OnDisable()
    {
        GameInputSystem.UnregisterInput();
        UIManager.UnregisterItemButtons();
    }


    private void Update()
    {
        //PreviewManager.UpdatePos();
    }
}
