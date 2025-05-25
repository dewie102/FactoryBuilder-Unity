using Assets.Scripts.EntitySystem;

using UnityEngine;

public class EntityView : MonoBehaviour
{
    public Entity Entity { get; private set; }

    public void Initialize(Entity entity)
    {
        Entity = entity;
    }
}
