using System.Linq;

using Assets.Scripts.EntitySystem;
using Assets.Scripts.EntitySystem.Interfaces;

using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public class EntityView : MonoBehaviour
    {
        [Header("Entity Sprite")]
        [SerializeField] private Transform entitySpriteTransform;

        [Header("Item Display")]
        [SerializeField] private Transform itemDisplayPoint;
        [SerializeField] private SpriteRenderer itemSpriteRenderer;

        private Entity _entity;
        private GameObject _currentItemVisual;

        public void Initialize(Entity entity)
        {
            _entity = entity;

            if(_entity is IItemProducer producer)
            {
                producer.ItemAdded += OnItemAdded;
                producer.ItemRemoved += OnItemRemoved;
            }

            if(_entity is IItemConsumer consumer)
            {
                consumer.ItemAdded += OnItemAdded;
                consumer.ItemRemoved += OnItemRemoved;
            }

            if(_entity is ItemHolderEntity holder)
                holder.DirectionsChanged += OnDirectionsChanged;

            ApplyOrientationVisual();
        }

        private void OnDestroy()
        {
            if(_entity is IItemProducer producer)
            {
                producer.ItemAdded -= OnItemAdded;
                producer.ItemRemoved -= OnItemRemoved;
            }

            if(_entity is IItemConsumer consumer)
            {
                consumer.ItemAdded -= OnItemAdded;
                consumer.ItemRemoved -= OnItemRemoved;
            }

            if(_entity is ItemHolderEntity holder)
                holder.DirectionsChanged -= OnDirectionsChanged;
        }

        private void OnDirectionsChanged()
        {
            ApplyOrientationVisual();
        }

        private void ApplyOrientationVisual()
        {
            Direction facing = Direction.RIGHT;

            if(_entity is IItemProducer producer && producer.OutputDirections.Any())
                facing = producer.OutputDirections.First();
            else if(_entity is IItemConsumer consumer && consumer.InputDirections.Any())
                facing = DirectionUtils.Reverse(consumer.InputDirections.First());

            float angle = facing switch
            {
                Direction.RIGHT => 0f,
                Direction.UP    => 90f,
                Direction.LEFT  => 180f,
                Direction.DOWN  => 270f,
                _               => 0f
            };

            entitySpriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);
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
                _currentItemVisual = Instantiate(item.prefab, displayPosition, Quaternion.identity, itemDisplayPoint);

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