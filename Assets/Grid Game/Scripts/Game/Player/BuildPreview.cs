
using UnityEngine;
using UtilSNR.Pool;

[System.Serializable]
public class BuildPreview
{
    private readonly int colorProp = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    private float opacity = 0.5f;
    
    private Transform currentPreviewObj = null;
    private Transform currentPreviewObjAnchor = null;
    private Vector3 anchorCenterOffset = Vector3.zero;

    private Material[] previewObjMaterials;
    
    public void SetPreview(GameObject previewItem)
    {
        if (previewItem == null)
            return;

        RecyclePreview();
        
        currentPreviewObj = PoolManager.Instance.Spawn(previewItem.transform);
        currentPreviewObjAnchor = GetAnchor(currentPreviewObj);
        anchorCenterOffset =  currentPreviewObj.position - currentPreviewObjAnchor.position;
        
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

    private void RecyclePreview()
    {
        if (currentPreviewObj == null)
            return;
        
        PoolManager.Instance.Despawn(currentPreviewObj);
        
        currentPreviewObj = null;
        currentPreviewObjAnchor = null;
        anchorCenterOffset = Vector3.zero;
        previewObjMaterials =  null;
    }
}
