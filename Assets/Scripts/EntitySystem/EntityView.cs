using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public class EntityView : MonoBehaviour
    {
        [Header("Item Display")]
        [SerializeField] private Transform itemDisplayPoint;
        [SerializeField] private SpriteRenderer itemSpriteRenderer;

        private Entity _entity;
        private GameObject _currentItemVisual;

        public void Initialize(Entity entity)
        {
            _entity = entity;

            // Subscribe to entity events
            if(_entity is IItemProducer producer)
            {
                // Listen for item changes on producers
                producer.ItemAdded += OnItemAdded;
                producer.ItemRemoved += OnItemRemoved;
            }

            if(_entity is IItemConsumer consumer)
            {
                // Listen for item changes on consumers
                consumer.ItemAdded += OnItemAdded;
                consumer.ItemRemoved += OnItemRemoved;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from entity events
            if(_entity is IItemProducer producer)
            {
                // Listen for item changes on producers
                producer.ItemAdded -= OnItemAdded;
                producer.ItemRemoved -= OnItemRemoved;
            }

            if(_entity is IItemConsumer consumer)
            {
                // Listen for item changes on consumers
                consumer.ItemAdded -= OnItemAdded;
                consumer.ItemRemoved -= OnItemRemoved;
            }
        }

        private void OnItemAdded(Item item)
        {
            ShowItem(item);
        }

        private void OnItemRemoved()
        {
            HideItem();
        }

        private void ShowItem(Item item)
        {
            if(_currentItemVisual != null)
            {
                Destroy(_currentItemVisual);
            }

            if(item?.prefab != null)
            {
                Vector3 displayPosition = itemDisplayPoint != null ? itemDisplayPoint.position : transform.position;
                _currentItemVisual = Instantiate(item.prefab, displayPosition, Quaternion.identity, transform);

                // Make it slightly smaller and offset so it doesn't hide the entity
                _currentItemVisual.transform.localScale = Vector3.one * 0.7f;
                _currentItemVisual.transform.localPosition = Vector3.up * 0.3f;
            }
        }

        private void HideItem()
        {
            if(_currentItemVisual != null)
            {
                Destroy(_currentItemVisual);
                _currentItemVisual = null;
            }
        }
    }
}