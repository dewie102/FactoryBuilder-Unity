using Assets.Scripts.EntitySystem;
using UnityEngine;

public struct ItemTransfer
{
    public Item item;
    public Vector3Int from;
    public Vector3Int to;
    public Entity fromEntity;
    public Entity toEntity;

    public ItemTransfer(Item item, Vector3Int from, Vector3Int to, Entity fromEntity = null, Entity toEntity = null)
    {
        this.item = item;
        this.from = from;
        this.to = to;
        this.fromEntity = fromEntity;
        this.toEntity = toEntity;
    }
}