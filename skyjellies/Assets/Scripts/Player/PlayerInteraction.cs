using Jellies;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class managing detection of interactble objects by the player
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    // The closest raycast hit
    private RaycastHit _closestHit = new RaycastHit();

    [SerializeField]
    private GameEventScriptableObject _onShowInteractionGUI;

    [SerializeField]
    private GameEventScriptableObject _onHideInteractionGUI;

    // How close the player must be to interact with an object
    [SerializeField]
    private float _interactionDistance = 3f;

    // The main camera transform
    [SerializeField]
    private Camera _mainCamera;

    // The first person camera transform
    [SerializeField]
    private Transform _fpsCamera;

    // The layers that should be checked for interaction
    [SerializeField]
    private LayerMask _interactableLayer;

    // The current interactable if any
    [SerializeField]
    private Interactable _interactable;

    private JellyInteractBase _jellyCurrent;
    
    // Jelly-Feeding
    [SerializeField] 
    private HotBar _hotBar;
    [SerializeField, Tooltip("The amount of food items to be consumed after feeding the jelly (units)")] 
    private int _foodConsumptionAmount = 1;    // can be removed/replaced

    /// <summary>
    /// If there is a raycast hit and no interactable assigned, enter interaction
    /// If there is a raycast hit and an interactable assigned, stay in interaction
    /// If there is not a raycast hit and an interactable assign, leave interaction
    /// </summary>
    private void Awake()
    {
        gameObject.GetComponent<PlayerInput>().onActionTriggered += HandleAction;

        _hotBar = GetComponent<HotBar>();
    }

    void Update()
    {
        if (CheckRaycast())
        {
            if (!_interactable)
            {
                OnInteractionEnter();
            }
            else
            {
                OnInteractionStay();
            }
        }
        else if (_interactable)
        {
            OnInteractionExit();
        }
    }

    /// <summary>
    /// Set the reference of this interactable to the raycast hit's, and enable the interactable
    /// </summary>
    private void OnInteractionEnter()
    {
        _interactable = _closestHit.collider.gameObject.GetComponent<Interactable>();
        _interactable.enabled = true;
        _onShowInteractionGUI.Raise();
        // Debug.Log("Interaction with " + closestHit.collider.gameObject + " enabled.");

        if (_interactable.gameObject.GetComponent<JellyInteractBase>() != null)
        {
            _jellyCurrent = _interactable.gameObject.GetComponent<JellyInteractBase>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnInteractionStay()
    {
        // Debug.Log("Interaction with " + interactable.gameObject + " continues.");
    }

    /// <summary>
    /// Disable the selected interactable, and set the reference to null
    /// </summary>
    private void OnInteractionExit()
    {
        _interactable.enabled = false;
        _onHideInteractionGUI.Raise();
        // Debug.Log("Interaction with " + interactable.gameObject + " disabled.");
        _interactable = null;
        _jellyCurrent = null;
    }

    /// <summary>
    /// Check for objects within interaction distance
    /// </summary>
    /// <returns>True if the raycast hits an object</returns>
    private bool CheckRaycast()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float distance = _interactionDistance + Vector3.Distance(_mainCamera.transform.position, _fpsCamera.position);
        return Physics.Raycast(ray, out _closestHit, distance, _interactableLayer, QueryTriggerInteraction.Ignore);
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (context.action.name == "Interact" && _jellyCurrent != null)
        {
            if (!_jellyCurrent.IsInteracting())
            {
                _jellyCurrent.InteractStart();
            }
        }
    }
    
    
    /// <summary>
    /// FeedingButton's OnClick listener.
    /// Check that the food quantity in the HotBar is sufficient (subtract _foodConsumptionAmount)
    /// </summary>
    /// <param name="itemInfo"></param>
    public void TryFeedJelly(ItemBase itemInfo)
    {
        if (!_hotBar.TrySubtractItemAmount(itemInfo, _foodConsumptionAmount))
        {
            print($"Not enough {itemInfo.name} to feed the jelly");
            return;
        }
        // Feed the jelly 
        _jellyCurrent.GetComponent<Feeding>().FeedJelly(itemInfo.SaturationValue);
    }
}