using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera currentCamera;

    public float panSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    public void HandlePanZoom()
    {
        Vector3 move = new();

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        currentCamera.transform.position += move * panSpeed * Time.deltaTime;

        float scroll = Mouse.current.scroll.ReadValue().y;
        currentCamera.orthographicSize = Mathf.Clamp(currentCamera.orthographicSize - scroll * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
    }
}
