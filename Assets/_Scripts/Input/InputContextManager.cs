using System;
using _Scripts.Input;
using _Scripts.Main.Services;
using UnityEngine;

public class InputContextManager : MonoBehaviour, IGameService
{
    private PlayerInputHandler _inputHandler;

    private void Awake()
    {
        _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
    }

    private void Start()
    {
        EnableGameplayControls();
    }

    public void EnableGameplayControls()
    {
        _inputHandler.InputActions.Gameplay.Enable();
        Debug.Log("[InputContextManager] Gameplay Controls Enabled");
    }

    public void EnableBuildControls()
    {
        DisableAll();
        _inputHandler.InputActions.Building.Enable();
        Debug.Log("[InputContextManager] Build Controls Enabled");
    }

    public void EnableUnitControls()
    {
        DisableAll();
        _inputHandler.InputActions.UnitCommand.Enable();
        Debug.Log("[InputContextManager] UnitCommand Controls Enabled");
    }

    private void DisableAll()
    {
        _inputHandler.InputActions.Building.Disable();
        _inputHandler.InputActions.UnitCommand.Disable();
    }
}