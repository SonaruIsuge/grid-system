
using SNR_PathFinding;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GridInstantiate : EditorWindow
{
    private const string GridInstantiateUxmlPath = "Assets/Grid Game/Scripts/Editor/GridInstantiate.uxml";
    
    private FloatField cellSizeInput;
    private UnsignedIntegerField widthInput;
    private UnsignedIntegerField heightInput;
    private Vector3Field offsetPositionInput;
    private Toggle autoCalcPositionToggle;
    private ObjectField penaltySettingInput;
    private Button searchExistSettingBtn;
    private Button createSettingBtn;
    private Toggle createDebugToggle;
    private Button createBoardBtn;

    private float cellSize;
    

    [MenuItem("Custom Grid System/Instantiate Game Board")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<GridInstantiate>();
        wnd.titleContent = new GUIContent("Grid Instantiate");

        wnd.minSize = new Vector2(350, 450);
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GridInstantiateUxmlPath);
        visualTree.CloneTree(rootVisualElement);
        
        InitFields();
        RegisterButtonEvent();

    }


    private void OnInspectorUpdate()
    {
        if (autoCalcPositionToggle.value)
        {
            var xPos = -(widthInput.value * cellSizeInput.value) / 2 + (cellSizeInput.value / 2);
            var zPos = -(heightInput.value * cellSizeInput.value) / 2 + (cellSizeInput.value / 2);
            offsetPositionInput.value = new Vector3(xPos, 0, zPos);
        }
    }


    private void InitFields()
    {
        cellSizeInput = rootVisualElement.Q<FloatField>("cell-size");
        widthInput = rootVisualElement.Q<UnsignedIntegerField>("width");
        heightInput = rootVisualElement.Q<UnsignedIntegerField>("height");
        offsetPositionInput = rootVisualElement.Q<Vector3Field>("offset-position");
        autoCalcPositionToggle = rootVisualElement.Q<Toggle>("auto-calculate-position-toggle");
        penaltySettingInput = rootVisualElement.Q<ObjectField>("penalty-setting");
        searchExistSettingBtn = rootVisualElement.Q<Button>("search-setting-btn");
        createSettingBtn = rootVisualElement.Q<Button>("create-setting-btn");
        createDebugToggle = rootVisualElement.Q<Toggle>("create-debug-toggle");
        createBoardBtn = rootVisualElement.Q<Button>("create-board-btn");
    }


    private void RegisterButtonEvent()
    {
        cellSizeInput.RegisterValueChangedCallback(SetCellSizeNonMinus);
        autoCalcPositionToggle.RegisterValueChangedCallback(OffsetPositionEnable);
        searchExistSettingBtn.clicked += SearchExistSetting;
        createSettingBtn.clicked += CreateSetting;
        createBoardBtn.clicked += CreateGameBoard;
    }


    private void SetCellSizeNonMinus(ChangeEvent<float> e)
    {
        cellSizeInput.value = Mathf.Max(e.newValue, 0f);
    }


    private void OffsetPositionEnable(ChangeEvent<bool> e)
    {
        offsetPositionInput.SetEnabled(!e.newValue);
    }


    private void SearchExistSetting()
    {
        var assets = AssetDatabase.FindAssets("t:TilePenaltySetting");
        
        if(assets == null) return;

        var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
        penaltySettingInput.value = AssetDatabase.LoadAssetAtPath<TilePenaltySetting>(assetPath);
    }


    private void CreateSetting()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Game Settings"))
            AssetDatabase.CreateFolder("Assets", "Game Settings");
        if(!AssetDatabase.IsValidFolder("Assets/Game Settings/Grid"))
            AssetDatabase.CreateFolder("Assets/Game Settings", "Grid");

        var setting = CreateInstance<TilePenaltySetting>();
        AssetDatabase.CreateAsset(setting, "Assets/Game Settings/Grid/TilePenaltySetting.asset");

        penaltySettingInput.value = setting;
    }
    

    private void CreateGameBoard()
    {
        var obj = new GameObject("Game Board");
        var gameBoard = obj.AddComponent<GameBoard>();
        if(gameBoard) gameBoard.InitBoardValue(cellSizeInput.value, (int)widthInput.value, (int)heightInput.value, offsetPositionInput.value,
            (TilePenaltySetting)penaltySettingInput.value);
        
        // Create debug object
        if (createDebugToggle.value)
        {
            var debugObj = new GameObject("Board Visual");
            var debugVisual = debugObj.AddComponent<PathDebugVisual>();
            debugVisual.SetGameBoard(gameBoard);
        }
    }
}
