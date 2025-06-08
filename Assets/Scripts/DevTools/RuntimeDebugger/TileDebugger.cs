using System;
using System.Text;
using Assets.Scripts.EntitySystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileDebugger : MonoBehaviour
{
    public GridManager gridManager;
    public EntityManager entityManager;
    public InputManager inputManager;
    public TextMeshProUGUI textMeshPro;

    private float _nextUpdate;
    public float updateRate = 0.1f; // 100 ms

    private GameObject tileHighlight;
    private SpriteRenderer highlightRenderer;

    public Color validColor = new Color(0f, 1f, 0f, 1.0f);   // Green transparent
    public Color invalidColor = new Color(1f, 0f, 0f, 1.0f); // Red transparent

    void Start()
    {
        tileHighlight = new();
        highlightRenderer = tileHighlight.AddComponent<SpriteRenderer>();

        highlightRenderer.sprite = Resources.Load<Sprite>("sprites/white_square");
        highlightRenderer.color = validColor;
        highlightRenderer.sortingOrder = 100; // Ensure it's on top

        tileHighlight.transform.localScale = Vector3.one; // Adjust if your tiles are scaled
    }

    void Update()
    {
        if (Time.time < _nextUpdate) return;
        _nextUpdate = Time.time + updateRate;

        StringBuilder sb = new();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int tilePos = gridManager.tilemaps[0].WorldToCell(mouseWorldPos);
        Vector3 cellCenter = gridManager.tilemaps[0].GetCellCenterWorld(tilePos);

        tileHighlight.transform.position = new Vector3(cellCenter.x, cellCenter.y, 0f);

        sb.Append($"Tile Position: {tilePos}\n");

        foreach (Tilemap tilemap in gridManager.tilemaps)
        {
            TileBase tile = tilemap.GetTile(tilePos);
            string tilename = tile != null ? tile.name : "";
            sb.Append($"{tilemap.name}: {tilename}\n");
        }

        Entity entity = entityManager.GetEntityAt(tilePos);
        string entityName = entity != null ? entity.Data.displayName : "";
        sb.Append($"Entity: {entityName}\n");

        textMeshPro.text = sb.ToString();

        highlightRenderer.color = entity != null ? invalidColor : validColor;
    }
}
