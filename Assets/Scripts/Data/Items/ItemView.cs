using UnityEngine;

public class ItemView : MonoBehaviour
{
    public Item Item { get; private set; }

    public void Initialize(Item item)
    {
        Item = item;
    }
}
