using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityLibrary", menuName = "FactoryGame/EntityLibrary")]
public class EntityLibrary : ScriptableObject
{
    [SerializeField]
    private List<EntityData> entities;

    private Dictionary<string, EntityData> _entityLookup;

    /// <summary>
    /// Should be called at startup to populate the lookup dictionary
    /// </summary>
    public void Initialize()
    {
        _entityLookup = entities.ToDictionary(e => e.id, e => e);
    }

    public EntityData GetById(string id)
    {
        if (_entityLookup == null)
            Initialize();

        _entityLookup.TryGetValue(id, out var data);
        return data;
    }

    public GameObject GetPrefab(string id)
    {
        var data = GetById(id);
        return data?.prefab;
    }

    public IEnumerable<EntityData> GetAll() => entities;
}
