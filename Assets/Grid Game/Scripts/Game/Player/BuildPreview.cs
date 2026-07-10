
using UnityEngine;
using UtilSNR.Pool;

[System.Serializable]
public class BuildPreview
{
    private readonly int colorProp = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private float opacity = 0.5f;
    
    private Transform currentPreviewObj = null;
    private Material[] previewObjMaterials;
    
    public void SetPreview(GameObject previewItem)
    {
        if (previewItem == null)
            return;

        ClearPreview();
        
        currentPreviewObj = PoolManager.Instance.Spawn(previewItem.transform);
        previewObjMaterials = currentPreviewObj.GetComponent<MeshRenderer>().materials;

        foreach (var mat in previewObjMaterials)
        {
            var color = mat.GetColor(colorProp);
            color.a = opacity;
            mat.SetColor(colorProp, color);
        }
    }

    public void UpdatePreview(Vector3 position, Quaternion rotation)
    {
        if (!currentPreviewObj)
            return;
        
        currentPreviewObj.position = position;
        currentPreviewObj.rotation = rotation;
    }
    
    private Transform GetAnchor(Transform previewObj)
    {
        var transforms = previewObj.GetComponentsInChildren<Transform>();
        return transforms.Length > 1 ? transforms[1] : transforms[0];
    }

    public void ClearPreview()
    {
        if (currentPreviewObj == null)
            return;
        
        PoolManager.Instance.Despawn(currentPreviewObj);
        
        currentPreviewObj = null;
        previewObjMaterials =  null;
    }
}
