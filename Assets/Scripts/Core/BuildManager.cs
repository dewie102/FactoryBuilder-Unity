using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem;

using UnityEngine;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Manages the building of entities in the game world.
    /// </summary>
    public class BuildManager : MonoBehaviour
    {
        [Header("Build Settings")]
        [SerializeField] private EntityData selectedEntity;
        [SerializeField] private LayerMask groundLayer = 1; // Optional: layer for valid build area

        [Header("Visual Feedback")]
        [SerializeField] private GameObject buildPreviewPrefab; // Optional; preview ghost
        private GameObject _currentPreview;
        private Direction _pendingOutputDirection = Direction.RIGHT;

        public EntityData SelectedEntity
        {
            get => selectedEntity;
            set
            {
                selectedEntity = value;
            }
        }

        public void HandleClick(Vector3 worldPosition)
        {
            if(SelectedEntity == null)
            {
                Debug.LogWarning("No entity selected for building.");
                return;
            }

            Vector3Int cellPosition = WorldManager.Instance.WorldToCell(worldPosition);

            if(!CanBuildAt(cellPosition))
            {
                Debug.Log($"Cannot build at {cellPosition}");
            }

            bool success = WorldManager.Instance.PlaceEntity(cellPosition, selectedEntity, _pendingOutputDirection);
            if(success)
                Debug.Log($"Successfully placed {selectedEntity.displayName} at {cellPosition}");
            else
            {
                Debug.LogWarning($"Failed to place {selectedEntity.displayName} at {cellPosition}");
            }
        }

        public void Rotate()
        {
            if(SelectedEntity == null)
            {
                Debug.LogWarning("No enitty selected for building.");
                return;
            }

            _pendingOutputDirection = DirectionUtils.GetRotatedDirection(_pendingOutputDirection);
            if(_currentPreview != null)
                _currentPreview.transform.Rotate(0.0f, 0.0f, 90.0f);
        }

        private bool CanBuildAt(Vector3Int cellPosition)
        {
            // Check if there's already an entity
            if(WorldManager.Instance.HasEntityAt(cellPosition))
            {
                Debug.Log("Cell already occupied");
                return false;
            }

            // Add any other build validation logic here
            // e.g., check if it's on valid terrain, within build bounds, etc.

            return true;
        }

        public void HandleHover(Vector3 worldPosition)
        {
            if(selectedEntity == null || buildPreviewPrefab == null) return;

            Vector3Int cellPosition = WorldManager.Instance.WorldToCell(worldPosition);
            Vector3 worldCellPosition = WorldManager.Instance.CellToWorld(cellPosition);

            if(_currentPreview == null)
                _currentPreview = Instantiate(buildPreviewPrefab, worldCellPosition, Quaternion.identity);
            else
                _currentPreview.transform.position = worldCellPosition;
        }

        public void ClearPreview()
        {
            if(_currentPreview != null)
            {
                Destroy(_currentPreview);
                _currentPreview = null;
                _pendingOutputDirection = Direction.RIGHT;
            }
        }

        public void SetSelectedEntity(EntityData entityData)
        {
            SelectedEntity = entityData;
            Debug.Log($"Selected entity for building: {entityData.displayName}");
            if(_currentPreview != null)
                Destroy(_currentPreview);

            buildPreviewPrefab = entityData.prefab;
        }
    }
}