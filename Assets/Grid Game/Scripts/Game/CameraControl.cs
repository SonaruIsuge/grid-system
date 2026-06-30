using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    
    [SerializeField] private CameraControlAttr attr;
    private CameraProp prop;

    private GameInputSystem Input => GameManager.Instance.GameInput;
    private Transform CamTrans => mainCam.transform;
    private Vector3 CurrentPos => mainCam.transform.position;
    private float CurrentFov => mainCam.fieldOfView;
    
    
    private void Start()
    {
        prop.Init(CurrentPos, CurrentFov);
    }

    private void Update()
    {
        UpdateProp();
        UpdateCamera();
    }


    private void UpdateProp()
    {
        prop.MoveDir = (CamTrans.forward * Input.Move.y + CamTrans.right * Input.Move.x);
        prop.MoveDir.y = 0;
        prop.TargetPos += prop.MoveDir * (attr.moveSpeed * Time.deltaTime);
        prop.IsMove = Input.Move != Vector2.zero;

        prop.RotateAngle = Input.Rotate * attr.rotateSpeed;
        prop.IsRotate = Input.Rotate != 0f;
        
        prop.TargetFov -= Input.Zoom * attr.zoomSpeed;
        prop.TargetFov = Mathf.Clamp(prop.TargetFov, attr.minZoomFov, attr.maxZoomFov);
        prop.IsZoom = Input.Zoom != 0f;
    }


    private void UpdateCamera()
    {
        
        CamTrans.position = Vector3.Lerp(CurrentPos, prop.TargetPos, attr.moveSmoothTime * Time.deltaTime);
        
        if (prop.IsRotate)
        {
            prop.TargetPos = CurrentPos;
            var plane = new Plane(Vector3.up, 0);
            var ray = new Ray(CamTrans.position, CamTrans.forward);
            plane.Raycast(ray, out var distance);
            var point = ray.GetPoint(distance);
            CamTrans.RotateAround(point, Vector3.up, -prop.RotateAngle * Time.deltaTime);
        }
        
        mainCam.fieldOfView = Mathf.Lerp(CurrentFov, prop.TargetFov, attr.zoomSmoothTime * Time.deltaTime);
    }
}

[Serializable]
public struct CameraProp
{
    public Vector3 TargetPos;
    public float TargetFov;
    public Vector3 MoveDir;
    public float RotateAngle;
    public bool IsMove;
    public bool IsRotate;
    public bool IsZoom;

    public void Init(Vector3 originPos, float originFov)
    {
        TargetPos = originPos;
        TargetFov = originFov;
        MoveDir = Vector3.zero;
        RotateAngle = 0f;
        IsMove = false;
        IsRotate = false;
        IsZoom = false;
    }
}

[Serializable]
public struct CameraControlAttr
{
    public float moveSpeed;
    public float moveSmoothTime;
        
    public float rotateSpeed;
        
    public float zoomSpeed;
    public float minZoomFov;
    public float maxZoomFov;
    public float zoomSmoothTime;
}