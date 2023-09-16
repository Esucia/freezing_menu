using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Player.View;

public class PauseManagement : MonoBehaviour
{
    private PlayerActions _pauseAction;
    private bool _paused = false;
    [SerializeField]
    private GameObject _soundSystem;
    [SerializeField]
    private CinemachineVirtualCamera[] _virtualCamera;
    [SerializeField]
    private FirstPersonView _firstPersonView;
    private bool _canPause;
    [SerializeField]
    private GameObject _jellyInteractVisual;


   private void Awake()
    {
        _pauseAction = new PlayerActions();
        _pauseAction.UI.Pause.performed += _ => DeterminePause();
        _paused = !_paused; 
        _soundSystem.SetActive(false);
        _firstPersonView.enabled = true;
        _canPause = true;
    }

    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
    }
    // <summary> 
    // Controls the state of the pause menu.
    // Has the ability to be disabled by other scripts if needed,
    // to prevent uneccesary bugs with movement and other UIs.
    // <summary>
    private void DeterminePause()
    {
        if(_canPause)
        {
            if (!_paused)
            {
                _paused = true;
                _soundSystem.SetActive(false);
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                foreach (var CinemachineVirtualCamera in _virtualCamera)
                {
                    CinemachineVirtualCamera.enabled = true;
                }
                _firstPersonView.enabled = true;
                _jellyInteractVisual.SetActive(true);

            }
            else
            {
                _paused = false;
                _soundSystem.SetActive(true);
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                foreach (var CinemachineVirtualCamera in _virtualCamera)
                {
                    CinemachineVirtualCamera.enabled = false;
                }
                _firstPersonView.enabled = false;
                _jellyInteractVisual.SetActive(false);
            }
        }
    }
    // <summary> 
    // Toggle the ability to pause for prevention of conflicts with jelly interaction.
    // <summary>
    public void TogglePause(bool state)
    {
        _canPause = state;
    }

}
