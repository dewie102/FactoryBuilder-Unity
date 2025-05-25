using Assets.Scripts.EntitySystem;

using UnityEngine;

public class VisualManager : MonoBehaviour
{
    private void OnEnable()
    {
        EntityManager.EntityPlaced += HandleEntityPlaced;
    }

    private void OnDisable()
    {
        EntityManager.EntityPlaced -= HandleEntityPlaced;
    }

    private void HandleEntityPlaced(Vector3Int position, Entity entity)
    {

    }
}
