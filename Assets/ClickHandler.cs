using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    InputAction leftMouseClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftMouseClick = InputSystem.actions.FindAction("MouseClick");
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMouseClick.WasPressedThisFrame())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Vector3 origin = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log($"Mouse Position: {mousePos} | Origin: {origin}");

            RaycastHit2D hit;
            hit = Physics2D.Raycast(origin, Vector3.forward, 1000);
            if (hit.collider != null)
            {
                print($"Found an Object: {hit.collider.GameObject().name} at distance: {hit.distance}");
                print($"Hit object at position: {hit.collider.gameObject.transform.position}");
            }
        }
    }

    public void OnPointerClick() {
        
    }
}
