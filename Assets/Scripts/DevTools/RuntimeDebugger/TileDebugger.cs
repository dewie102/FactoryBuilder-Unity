using System;
using System.Text;

using Assets.Scripts.Core;
using Assets.Scripts.EntitySystem;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DevTools.RuntimeDebugger
{
    public class TileDebugger : MonoBehaviour
    {
        [Header("References")]
        public InputManager inputManager;
        public TextMeshProUGUI textMeshPro;

        [Header("Settings")]
        public float updateRate = 0.1f; // 100 ms
        public Color validColor = new Color(0f, 1f, 0f, 1.0f);   // Green transparent
        public Color invalidColor = new Color(1f, 0f, 0f, 1.0f); // Red transparent

        private float _nextUpdate;
        private GameObject _tileHighlight;
        private SpriteRenderer _highlightRenderer;

        void Start()
        {
            CreateTileHighlight();
        }

        void Update()
        {
            if(Time.time < _nextUpdate) return;
            _nextUpdate = Time.time + updateRate;

            UpdateDebugInfo();
        }

        private void CreateTileHighlight()
        {
            _tileHighlight = new GameObject("TileHighlight");
            _highlightRenderer = _tileHighlight.AddComponent<SpriteRenderer>();

            _highlightRenderer.sprite = Resources.Load<Sprite>("sprites/white_square");
            _highlightRenderer.color = validColor;
            _highlightRenderer.sortingOrder = 100; // Ensure it's on top

            _tileHighlight.transform.localScale = Vector3.one;
        }

        private void UpdateDebugInfo()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int tilePosition = WorldManager.Instance.WorldToCell(mouseWorldPosition);

            // Update Highlight position
            UpdateHighlightPosition(tilePosition);

            // Build debug text
            StringBuilder sb = new();
            sb.Append($"Tile Position: {tilePosition}\n");

            // Get tile information from WorldManager
            sb.Append(WorldManager.Instance.GetTileDebugInfo(tilePosition));

            // Get entity information
            Entity entity = WorldManager.Instance.GetEntityAt(tilePosition);
            string entityName = entity != null ? entity.Data.displayName : "None";
            sb.Append($"Entity: {entityName}\n");

            textMeshPro.text = sb.ToString();

            // Update highlight color based on entity presence
            _highlightRenderer.color = entity != null ? invalidColor : validColor;
        }

        private void UpdateHighlightPosition(Vector3Int tilePosition)
        {
            Vector3 worldPosition = WorldManager.Instance.CellToWorld(tilePosition);
            _tileHighlight.transform.position = worldPosition;
        }

        private void OnDestroy()
        {
            if (_tileHighlight != null)
            {
                Destroy(_tileHighlight);
            }
        }
    }
}