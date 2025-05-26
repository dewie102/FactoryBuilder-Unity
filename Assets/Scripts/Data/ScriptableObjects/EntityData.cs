using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.EntitySystem
{
    public enum EntityCategory
    {
        Production,
        Logistics,
        Resource,
        Utility,
        Other
    }

    public enum EntityType
    {
        Conveyor,
        Machine,
        ResourceNode
    }

    [CreateAssetMenu(fileName = "NewEntityData", menuName = "FactoryGame/Entity Data")]
    public class EntityData: ScriptableObject
    {
        [Header("Basic Info")]
        public string id; // Unique string ID (e.g. "miner_basic")
        public string displayName; // UI Display Name (e.g. "Basic Miner")
        public Sprite icon; // For UI build menu

        [Header("Classification")]
        public EntityCategory category; // Optoinal enum to group types
        public EntityType type; // For logic and factory use

        [Header("Placement Info")]
        public GameObject prefab; // Visual prefab to spawn
        public Vector2Int size = Vector2Int.one; // Default to 1x1 tile
        public int cost;

        [Header("Logic")]
        public float buildTime = 0f;
        public bool isPassThrough = false; // e.g. conveyors
        public bool requiresPower = false; // e.g. factories       
    }
}
