using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    InputAction leftMouseClick;
    Tilemap myTileMap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftMouseClick = InputSystem.actions.FindAction("MouseClick");
        myTileMap = FindFirstObjectByType<Tilemap>();
        myTileMap = GameObject.Find("Ground").GetComponent<Tilemap>();
        print(myTileMap.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMouseClick.WasPressedThisFrame())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Vector3 origin = Camera.main.ScreenToWorldPoint(mousePos);
            origin.z = myTileMap.transform.position.z;

            Vector3Int originInt = new Vector3Int((int)origin.x, (int)origin.y, (int)origin.z);
            TileBase myTile = myTileMap.GetTile(originInt);
            print($"Tile: {myTile} | Position: {originInt}");
        }
    }

    public void OnPointerClick() {
        
    }
}
