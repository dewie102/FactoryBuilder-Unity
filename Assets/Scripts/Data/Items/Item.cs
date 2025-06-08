using System;
using UnityEngine;

public class Item : IEquatable<Item>
{
    public string ID { get; private set; }
    public string DisplayName { get; private set; }
    public Sprite sprite;
    public int maxStackSize; // for inventory systems
    public ItemCategory category;
    public int value; // for economy systems later?

    public bool Equals(Item other)
    {
        return other != null && ID == other.ID;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Item);
    }

    public override int GetHashCode()
    {
        return ID != null ? ID.GetHashCode() : 0;
    }

    public static bool operator ==(Item left, Item right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Item left, Item right)
    {
        return !(left == right);
    }
}
