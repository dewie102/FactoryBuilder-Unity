using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    public float tickInterval = 2f;
    private float _timer;

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= tickInterval)
        {
            _timer = 0f;
            TickAllEntities();
        }
    }

    private void TickAllEntities()
    {
        Debug.Log("SimulationManager Tick");
        EntityManager.Instance.TickEntities();
    }
}
