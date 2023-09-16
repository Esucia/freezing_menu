using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Provides for straight-line movements in the forward/backward and strafing left/right.
    /// </summary>
    /// <inheritdoc cref="Movement" path="/remarks/"/>
    public class RectilinearMovement : Movement
    {
        private Vector2 _inputDirection; // cache the input vector for use when updating the movement direction

        /// <inheritdoc/>
        /// <summary>Motion is a straight-line direction relative to which way the character is facing.</summary>
        protected override Vector3 Direction
        {
            get
            {
                Transform currentTransform = transform;
                return (currentTransform.right * _inputDirection.x) + (currentTransform.forward * _inputDirection.y);
            }
        }

        /// <inheritdoc/>
        public override void Go(Vector2 move)
        {
            _inputDirection = move;
            TargetSpeed = IsSprinting ? sprintSpeed : walkSpeed;
        }
    }
}
