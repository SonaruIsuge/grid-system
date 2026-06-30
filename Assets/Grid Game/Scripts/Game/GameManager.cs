using System;
using SNR_BuildSystem;
using UnityEngine;
using UtilSNR.Common;

public class GameManager : TSceneSingletonBehaviour<GameManager>
{
    private GameInputSystem gameInput;

    public GameBoard GameBoard;
    public GridBuildManager GridBuildManager;
    public PreviewManager PreviewManager;
    public UIManager UIManager;

    public GameInputSystem GameInput => gameInput;


    protected override void Awake()
    {
        base.Awake();

        gameInput = new();
    }


    private void OnEnable()
    {
        gameInput.RegisterInput();
        UIManager.RegisterItemButtons(GridBuildManager.TiledItemList);
    }


    private void OnDisable()
    {
        gameInput.UnregisterInput();
        UIManager.UnregisterItemButtons();
    }

    private void Start()
    {
        // Set up managers
        if (PreviewManager != null) PreviewManager.Setup(GameBoard.Grid, gameInput);
    }


    private void Update()
    {
        //PreviewManager.UpdatePos();
    }
}
