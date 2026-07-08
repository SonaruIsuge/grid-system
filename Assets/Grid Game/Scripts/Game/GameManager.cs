using System;
using SNR_BuildSystem;
using UnityEngine;
using UtilSNR.Common;

public class GameManager : TSceneSingletonBehaviour<GameManager>
{
    private GameInputSystem gameInput;

    public GameBoard GameBoard;
    public UIManager UIManager;

    public GameInputSystem GameInput => gameInput;


    protected override void Awake()
    {
        base.Awake();

        gameInput = new GameInputSystem();
    }


    private void OnEnable()
    {
        gameInput.RegisterInput();
        UIManager.RegisterItemButtons(GridBuildManager.Instance.TiledItemList);
    }


    private void OnDisable()
    {
        gameInput.UnregisterInput();
        UIManager.UnregisterItemButtons();
    }

    private void Start()
    {
        
    }
}
