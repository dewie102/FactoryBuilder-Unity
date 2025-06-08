using UnityEngine;

namespace Assets.Scripts.Data.Items
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "FactoryGame/Item Data")]
    public class ItemData: ScriptableObject
    {
        [Header("Basic Info")]
        public string id; // Unique string ID (e.g. "miner_basic")
        public string displayName; // UI Display Name (e.g. "Basic Miner")
        public Sprite sprite;
        public int maxStackSize;

        [Header("Classification")]
        public ItemCategory category; // Optoinal enum to group types

        [Header("Placement Info")]
        public GameObject prefab; // Visual prefab to spawn
        public Vector2Int size = Vector2Int.one; // Default to 1x1 tile
        public int cost;      
    }
}