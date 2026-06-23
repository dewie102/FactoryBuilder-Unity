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
        private WorldManager worldManager;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            worldManager = WorldManager.Instance;
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
                        worldManager.SelectEntity(mouseWorldPos);
                    if(Keyboard.current.rKey.wasPressedThisFrame)
                        worldManager.RotateEntity(mouseWorldPos);
                    break;
                case GameInputState.DeleteMode:
                    if(Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        Vector3Int cellPosition = worldManager.WorldToCell(mouseWorldPos);
                        worldManager.RemoveEntity(cellPosition);
                    }
                    break;
            }

            if(Keyboard.current.bKey.wasPressedThisFrame)
                SetState(GameInputState.BuildMode);
            else if(Keyboard.current.pKey.wasPressedThisFrame)
                SetState(GameInputState.SelectMode);
            else if(Keyboard.current.xKey.wasPressedThisFrame)
                SetState(GameInputState.DeleteMode);
        }

        public void SetState(GameInputState newState)
        {
            Debug.Log($"Changing States: Old State: {currentState} | New State: {newState}");
            currentState = newState;
        }
    }
}