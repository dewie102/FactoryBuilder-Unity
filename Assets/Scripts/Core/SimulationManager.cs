using UnityEngine;

namespace Assets.Scripts.Core
{
    public class SimulationManager : MonoBehaviour
    {
        public static SimulationManager Instance { get; private set; }

        public bool simulationActive = true;
        public float tickInterval = 2f;
        private float _timer;

        public void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            if(!simulationActive)
                return;

            _timer += Time.deltaTime;
            if(_timer >= tickInterval)
            {
                _timer = 0f;
                TickAllEntities();
            }
        }

        public void TickAllEntities()
        {
            Debug.Log("SimulationManager Tick");
            WorldManager.Instance.TickWorld();
        }
    }
}