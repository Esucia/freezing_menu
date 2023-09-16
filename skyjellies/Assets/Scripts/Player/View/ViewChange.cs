using System;
using Cinemachine;
using Player.Motion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.View
{
    /// <summary>
    /// Provides a smooth transition between first- and third-person camera views.
    /// </summary>
    /// <remarks>
    /// \todo Explore other implementations that might be more flexible
    /// This implementation depends on a specific camera setup involving a CinemachineMixingCamera with three vcams:
    /// first-person, third-person near, and third-person far.
    /// Additionally, the active action map of the <c>PlayerInput</c> component must have an input action named "Zoom"
    /// that encapsulates a <c>float</c> value. Note that this component will fail gracefully (and silently) if the
    /// required input actions are not found.
    /// </remarks>
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(FirstPersonView))]
    [RequireComponent(typeof(ThirdPersonView))]
    [RequireComponent(typeof(RectilinearMovement))]
    [RequireComponent(typeof(ForwardMovement))]
    public class ViewChange : InputHandler
    {
        public static Action<ViewController> OnViewChanged;
        
        [SerializeField,Tooltip("Uses Cinemachine Mixing camera to blend between first and third-person views")]
        private CinemachineMixingCamera cameraBlend;
        [FormerlySerializedAs("_zoomSensitivity")] [SerializeField,Range(0,1),Tooltip("Use small values for slower zoom, large values for faster zoom")]
        private float zoomSensitivity = 0.5f;

        private FirstPersonView _firstPersonView;
        private ThirdPersonView _thirdPersonView;
        private ForwardMovement _thirdPersonMovement;
        private RectilinearMovement _firstPersonMovement;

        private void Awake()
        {
            _firstPersonView = GetComponent<FirstPersonView>();
            _firstPersonMovement = GetComponent<RectilinearMovement>();
            _thirdPersonView = GetComponent<ThirdPersonView>();
            _thirdPersonMovement = GetComponent<ForwardMovement>();
            zoomSensitivity = 360 * (1 + zoomSensitivity);
        }

        /// <summary>
        /// Adjust the field-of-view to smoothly change between third- and first-person views.
        /// </summary>
        /// <remarks>
        /// \todo Implement camera zoom to/from first-person
        /// We separate this from the <c>OnZoom</c> handler  and expose it publicly so that zoom in/out can be
        /// triggered programmatically if desired.
        /// </remarks>
        public void Zoom(float fov)
        {
            if (cameraBlend.m_Weight2 == 0 && fov >= 0)
            {
                
                cameraBlend.m_Weight0 = 1;
                cameraBlend.m_Weight1 = 0;
                cameraBlend.m_Weight2 = 0;
            }
            else if (_firstPersonView.enabled && fov < 0)
            {
                cameraBlend.m_Weight0 = 0;
                cameraBlend.m_Weight1 = 0.99f;
                cameraBlend.m_Weight2 = 0.01f;
            }
            else
            {
                cameraBlend.m_Weight0 = 0;
                cameraBlend.m_Weight1 = math.clamp(cameraBlend.m_Weight1 + fov, 0, 1);
                cameraBlend.m_Weight2 = math.clamp(cameraBlend.m_Weight2 - fov, 0, 1);
            }
            if (cameraBlend.m_Weight0 >= 1) // TODO fix floating-point comparison using tolerance epsilon
            {
                if (_thirdPersonView.enabled)
                {
                    _thirdPersonMovement.enabled = false;
                    _thirdPersonView.enabled = false;
                    _firstPersonView.enabled = true;
                    _firstPersonMovement.enabled = true;
                    OnViewChanged?.Invoke(_firstPersonView);
                }
            }
            else if (_firstPersonView.enabled)
            {
                _firstPersonMovement.enabled = false;
                _firstPersonView.enabled = false;
                _thirdPersonView.enabled = true;
                _thirdPersonMovement.enabled = true;
                OnViewChanged?.Invoke(_thirdPersonView);
            }
        }

        /// <summary>
        /// Handles "Zoom" input actions triggered by the input system.
        /// </summary>
        /// <param name="context">
        /// should represent a "Value" action with <c>float</c> control type
        /// </param>
        /// <remarks>
        /// While there are several ways to obtain the zoom control value, we use "Passthrough" for the Y-component of
        /// the mouse scroll <c>Vector2</c>.
        /// </remarks>
        private void OnZoom(InputAction.CallbackContext context)
        {
            Zoom(context.ReadValue<float>() / zoomSensitivity);
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Zoom" input actions.</remarks>
        protected override void RegisterEventHandlers(PlayerInput input)
        {
            InputAction zoomAction = input.actions.FindAction("Zoom");
            if (zoomAction != null)
            {
                zoomAction.performed += OnZoom;
            }
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Zoom" input actions.</remarks>
        protected override void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction zoomAction = input.actions.FindAction("Zoom");
            if (zoomAction != null)
            {
                zoomAction.performed -= OnZoom;
            }
        }
    }
}
