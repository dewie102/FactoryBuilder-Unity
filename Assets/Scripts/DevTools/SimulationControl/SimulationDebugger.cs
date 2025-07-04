using System.Text;

using Assets.Scripts.Core;

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimulationDebugger : MonoBehaviour
{
    public Toggle autoSimulationToggle;
    public Slider tickIntervalSlider;
    public TextMeshProUGUI debugText;

    bool autoSimulate = true;
    float tickInterval = 1f;
    float tickTimer = 0f;
    int tickCount = 0;

    void Start()
    {
        autoSimulationToggle.onValueChanged.AddListener(SetSimulationMode);
        tickIntervalSlider.onValueChanged.AddListener(SetTickInterval);

        autoSimulationToggle.isOn = autoSimulate;
        tickIntervalSlider.value = tickInterval;
    }

    void Update()
    {
        if (autoSimulate)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickInterval)
            {
                TriggerTick();
                tickTimer = 0f;
            }
        }
        else
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TriggerTick();
            }
        }

        UpdateDebugText();
    }

    public void TriggerTick()
    {
        WorldManager.Instance.TickWorld();
        tickCount++;
    }

    public void SetSimulationMode(bool autoMode)
    {
        autoSimulate = autoMode;
        SimulationManager.Instance.simulationActive = autoSimulate;
    }

    public void SetTickInterval(float newInterval)
    {
        tickInterval = newInterval;
    }

    private void UpdateDebugText()
    {
        StringBuilder sb = new();

        string simulationMode = autoSimulate ? "Auto" : "Manual";
        sb.Append($"Simulation Mode: {simulationMode}\n");
        sb.Append($"Tick Count: {tickCount}\n");
        sb.Append($"Tick Interval: {tickInterval}");

        debugText.text = sb.ToString();
    }
}
