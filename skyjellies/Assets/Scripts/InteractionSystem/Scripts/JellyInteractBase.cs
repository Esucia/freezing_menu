using Jellies;
using UnityEngine;
using Jellies.Behaviors;
using Player.View;
using Player.Motion;
using Player;
using System;
using TMPro;
using UnityEngine.UI;

public class JellyInteractBase : Interactable
{
    [Header("Timing")]
    [SerializeField]
    private float _reactivateTime = 1.25f;

    [Header("Controlled Objects")]
    [SerializeField, Tooltip("UI on the jelly to be used to handle interaction.")]
    private GameObject _interactionUI;
    [SerializeField, Tooltip("Player UI to hide during interaction.")]
    private GameObject _inventoryUI;
    [SerializeField, Tooltip("Player camera script to prevent movement of the camera when not needed.")]
    private FirstPersonView _firstPersonView;
    [SerializeField, Tooltip("Virtual Jelly camera to focus for interaction.")]
    private GameObject _jellyVCamera;
    [SerializeField, Tooltip("Movement on the player to be used to handle movement.")]
    private RectilinearMovement _movementPlayer;

    [SerializeField, Tooltip("Controls whether player can pause or not, as well as the ability to pause")]
    private PauseManagement _pauseManagement;

    private bool _interacting;
    private Wandering _pathing;
    
    // Feed-Jelly interaction UI
    [SerializeField, Tooltip("If the jelly is full, disable the feedButton")] 
    private Button _feedButton;
    [SerializeField] 
    private Slider _saturationSlider;
    [SerializeField] 
    private TextMeshProUGUI _saturationText;
    
    private Parameters _jellyParams;
    
    // Start is called before the first frame update
    void Start()
    {
        _pathing = this.gameObject.GetComponent<Wandering>();
        InteractStop(); // Ensure everything is in default state to start

        _jellyParams = GetComponent<Parameters>();
    }

    private void Update()
    {
        if (_interacting)
        {
            OnInteractionStay();
        }
    }

    // <summary> 
    // Deactivate wandering and parts of player, in preparation for activating interaction variables. Prevents the
    // Player from moving and sliding around while in interaction since a sudden disabling of movement causes constant
    // application of velocity.
    // <summary>
    public void InteractStart()
    {
        _interacting = true;
        _pathing.ToggleWander(false);

        _jellyVCamera.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        _interactionUI.SetActive(false);
        _inventoryUI.SetActive(false);
        _firstPersonView.enabled = false;

        _movementPlayer.Go(Vector2.zero);
        _movementPlayer.gameObject.GetComponent<Body>().enabled = false;
        _movementPlayer.gameObject.GetComponent<CharacterController>().Move(Vector3.zero);
        _movementPlayer.enabled = false;

        // Debug.Log("Interact");
        _pauseManagement.TogglePause(false);
    }
    
    /// <summary>
    /// During the interaction duration (when _interacting == true)
    /// </summary>
    private void OnInteractionStay()
    {
        _feedButton.interactable = _jellyParams.FoodSaturation < _jellyParams.MaxFoodSaturation;    // Disable Feed-button when jelly is full
        _saturationSlider.value = _jellyParams.FoodSaturation;
        _saturationText.SetText($"Saturation: {_jellyParams.FoodSaturation}");
    }
    
    // <summary> 
    // Reactivate wandering and parts of player, in preparation for returning to normal gameplay.
    // <summary>
    public void InteractStop()
    {
        _interacting = false;
        _pathing.ToggleWander(true); 

        _jellyVCamera.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Invoke(nameof(ReactivateUI), _reactivateTime);
        // Debug.Log("Interact Stop");
    }

    // <summary>
    // Hold variables to reactivate when Invoked, preventing visual glitches.
    // <summary>
    void ReactivateUI()
    {
        _interactionUI.SetActive(true);
        _inventoryUI.SetActive(true);
        _firstPersonView.enabled = true;
        _movementPlayer.enabled = true;

        _movementPlayer.gameObject.GetComponent<Body>().enabled = true;

        _pauseManagement.TogglePause(true);
    }

    // <summary>  
    // Relay current state of interaction for the player interaction
    // <summary>
    public bool IsInteracting()
    {
        return _interacting;
    }

}
