
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputSystem
{
    private UserInputControl inputControl;
    public Vector2 MousePosition { get; private set; }
    public bool LeftMouseDown => inputControl.InGame.LeftMouse.WasPerformedThisFrame() && inputControl.InGame.LeftMouse.ReadValue<float>() != 0;
    public Vector2 Move { get; private set; }
    public float Zoom { get; private set; }
    public float Rotate { get; private set; }
    public bool RotateObj => inputControl.InGame.RotateObj.WasPressedThisFrame();

    public GameInputSystem()
    {
        inputControl = new UserInputControl();
        inputControl.InGame.Enable();
    }

    public void RegisterInput()
    {
        inputControl.InGame.MousePosition.performed += OnMousePositionPerformed;
        inputControl.InGame.Move.performed += OnMovePerformed;
        inputControl.InGame.Move.canceled += OnMoveCanceled;
        inputControl.InGame.Zoom.performed += OnZoomPerformed;
        inputControl.InGame.Zoom.canceled += OnZoomCanceled;
        inputControl.InGame.Rotate.performed += OnRotatePerformed;
        inputControl.InGame.Rotate.canceled += OnRotateCanceled;
    }


    public void UnregisterInput()
    {
        inputControl.InGame.MousePosition.performed -= OnMousePositionPerformed;
        inputControl.InGame.Move.performed -= OnMovePerformed;
        inputControl.InGame.Move.canceled -= OnMoveCanceled;
        inputControl.InGame.Zoom.performed -= OnZoomPerformed;
        inputControl.InGame.Zoom.canceled -= OnZoomCanceled;
        inputControl.InGame.Rotate.performed -= OnRotatePerformed;
        inputControl.InGame.Rotate.canceled -= OnRotateCanceled;
    }


    private void OnMousePositionPerformed(InputAction.CallbackContext ctx)
    {
        MousePosition = ctx.ReadValue<Vector2>();
    }


    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }


    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        Move = Vector2.zero;
    }


    private void OnZoomPerformed(InputAction.CallbackContext ctx)
    {
        Zoom = ctx.ReadValue<Vector2>().y;
    }


    private void OnZoomCanceled(InputAction.CallbackContext ctx)
    {
        Zoom = 0f;
    }


    private void OnRotatePerformed(InputAction.CallbackContext ctx)
    {
        Rotate = ctx.ReadValue<float>();
    }


    private void OnRotateCanceled(InputAction.CallbackContext ctx)
    {
        Rotate = 0f;
    }
}
