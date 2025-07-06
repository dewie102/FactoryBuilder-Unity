using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private CameraController cameraController;
        [SerializeField] private BuildManager buildManager;

        private GameInputState currentState = GameInputState.SelectMode;
        private Vector3 mouseWorldPos;

        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            cameraController.HandlePanZoom();

            mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            switch(currentState)
            {
                case GameInputState.BuildMode:
                    if(Mouse.current.leftButton.wasPressedThisFrame)
                        buildManager.HandleClick(mouseWorldPos);
                    break;
                case GameInputState.SelectMode:
                    if(Mouse.current.leftButton.wasPressedThisFrame)
                        HandleSelect(mouseWorldPos);
                    break;
            }

            if(Keyboard.current.bKey.wasPressedThisFrame)
                SetState(GameInputState.BuildMode);
            else if(Keyboard.current.pKey.wasPressedThisFrame)
                SetState(GameInputState.SelectMode);
        }

        public void SetState(GameInputState newState)
        {
            Debug.Log($"Changing States: Old State: {currentState} | New State: {newState}");
            currentState = newState;
        }

        private void HandleSelect(Vector3 pos)
        {
            // Do some selection logic?
            Debug.Log($"Selected at: {pos}");
        }
    }
}