
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class UIBlockProcessor : InputProcessor<float>
{
    private const string ProcessorName = "UIBlockProcessor";
    
    
#if UNITY_EDITOR
    static UIBlockProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if(InputSystem.TryGetProcessor(ProcessorName) != null)
            return;
        
        InputSystem.RegisterProcessor<UIBlockProcessor>(ProcessorName);
    }
    
    
    
    public override float Process(float value, InputControl control)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return 0;
        }

        return value;
    }
}
