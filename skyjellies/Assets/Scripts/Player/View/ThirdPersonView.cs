using UnityEngine;

namespace Player.View
{
    /// <summary>
    /// Basic character-follow behavior for use with a third-person camera.
    /// </summary>
    /// <inheritdoc/>
    public class ThirdPersonView : ViewController
    {
        /// <summary>
        /// Additional degrees to override the camera (useful for fine tuning camera position when locked).
        /// </summary>
        [SerializeField
         , Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        protected float cameraAngleOverride;

        /// <summary>
        /// When <c>true</c>, locks the camera orientation on all axes.
        /// </summary>
        [SerializeField] [Tooltip("For locking the camera position on all axis")]
        protected bool lockCameraPosition;

        protected void Start()
        {
            cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Moves the cinemachine virtual camera in a circular orbit around the follow target.
        /// </remarks>
        protected override void TurnCamera()
        {
            if (lookDirection.sqrMagnitude > LookThreshold && !lockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                cinemachineTargetYaw   += lookDirection.x * deltaTimeMultiplier;
                cinemachineTargetPitch += lookDirection.y * deltaTimeMultiplier;
                cinemachineTargetYaw   =  ClampAngle(cinemachineTargetYaw,   float.MinValue, float.MaxValue);
                cinemachineTargetPitch =  ClampAngle(cinemachineTargetPitch, bottomClamp,    topClamp);
            }

            cinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
        }
    }
}
