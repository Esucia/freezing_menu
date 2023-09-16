using UnityEngine;

namespace Player.View
{
    /// <summary>
    /// Basic look-around behavior for use with a first-person camera.
    /// </summary>
    /// <inheritdoc/>
    public class FirstPersonView : ViewController
    {
        /// <inheritdoc />
        /// <remarks>
        /// Pivots the cinemachine virtual camera and the character to follow the pointer input.
        /// </remarks>
        protected override void TurnCamera()
        {
            if (lookDirection.sqrMagnitude < LookThreshold) { return; }

            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            float rotationVelocity = lookDirection.x * deltaTimeMultiplier;
            cinemachineTargetPitch += lookDirection.y * deltaTimeMultiplier;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * rotationVelocity);
        }

        public Transform SetTarget(Transform newTarget)
        {
            GameObject oldTrans = cinemachineCameraTarget;

            cinemachineCameraTarget = newTarget.gameObject;

            return oldTrans.transform;
        }
    }
}