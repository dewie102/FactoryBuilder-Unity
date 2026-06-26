using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core;
using Assets.Scripts.Data.Entities;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

namespace Assets.Scripts.EntitySystem.Logistics
{
    public class ConveyorEntity : ItemHolderEntity, IItemConsumer, IItemProducer, IChainableEntity
    {
        private readonly HashSet<Direction> _inputDirections = new();
        private readonly HashSet<Direction> _outputDirections = new();

        public ConveyorEntity(EntityData data) : base(data)
        {
            SetOrientation(Direction.LEFT, Direction.RIGHT);
        }

        public IEnumerable<Direction> InputDirections => _inputDirections;
        public IEnumerable<Direction> OutputDirections => _outputDirections;

        public override void OnTick()
        {
            Debug.Log($"{this} is ticking.");
        }

        public override bool CanConsumeItem(Item item)
        {
            //return !HasItem;
            bool canConsume = !HasItem;
            Debug.Log($"ConveyorEntity.CanConsumeItem: HasItem={HasItem}, CanConsume={canConsume}");
            return canConsume;
        }

        public override void OnPlaced(Direction outputDirection, Dictionary<Direction, Entity> neighbors)
        {
            ApplySmartInputSnap(outputDirection, neighbors);
        }

        public override void Rotate(Dictionary<Direction, Entity> neighbors)
        {
            Direction newOutputDirection = DirectionUtils.GetRotatedDirection(OutputDirections.First());
            ApplySmartInputSnap(newOutputDirection, neighbors);
        }

        private void ApplySmartInputSnap(Direction desiredOutput, Dictionary<Direction, Entity> neighbors)
        {
            Direction newInputDirection = DirectionUtils.Reverse(desiredOutput);

            foreach(KeyValuePair<Direction, Entity> neighbor in neighbors)
            {
                if(neighbor.Key == desiredOutput)
                    continue;

                if(neighbor.Value is IItemProducer neighborProducer && neighborProducer.OutputDirections.Contains(DirectionUtils.Reverse(neighbor.Key)))
                {
                    newInputDirection = neighbor.Key;
                    break;
                }
            }

            SetOrientation(newInputDirection, desiredOutput);
            Debug.Log($"ConveyorEntity.ApplySmartInputSnap: input={newInputDirection} | output={desiredOutput}");
        }

        public void SetOrientation(Direction input, Direction output)
        {
            _inputDirections.Clear();
            _outputDirections.Clear();

            _inputDirections.Add(input);
            _outputDirections.Add(output);

            NotifyDirectionsChanged();
        }

        public override void SetInputDirection(Direction direction)
        {
            SetOrientation(direction, _outputDirections.First());
        }

        public override void SetOutputDirection(Direction direction)
        {
            SetOrientation(DirectionUtils.Reverse(direction), direction);
        }
    }
}
