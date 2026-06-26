using System.Linq;

using Assets.Scripts.Data.Entities;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private CameraController cameraController;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private EntityLibrary entityLibrary;

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
                    buildManager.HandleHover(mouseWorldPos);
                    if(Mouse.current.leftButton.wasPressedThisFrame)
                        buildManager.HandleClick(mouseWorldPos);
                    if(Keyboard.current.rKey.wasPressedThisFrame)
                        buildManager.Rotate();
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

            HandleEntityHotkeys();
        }

        private void HandleEntityHotkeys()
        {
            var allEntities = entityLibrary.GetAll().ToArray();
            for(int i = 0; i < Mathf.Min(allEntities.Length, 9); i++)
            {
                if(Keyboard.current[(Key)(Key.Digit1 + i)].wasPressedThisFrame)
                {
                    buildManager.SetSelectedEntity(allEntities[i]);
                    SetState(GameInputState.BuildMode);
                    break;
                }
            }
        }

        public void SetState(GameInputState newState)
        {
            Debug.Log($"Changing States: Old State: {currentState} | New State: {newState}");
            if(currentState == GameInputState.BuildMode)
                buildManager.ClearPreview();
            currentState = newState;
        }
    }
}