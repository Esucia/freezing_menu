using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Provides forward movement oriented relative to the camera view, best used with a third-person perspective.
    /// </summary>
    /// <inheritdoc cref="Movement"/>
    public class ForwardMovement : Movement
    {
        /// <summary>
        /// Defines which layers the player collides with for the purpose of determining grounded state.
        /// </summary>
        [SerializeField,
         Tooltip("Reference to the main camera in the scene. If blank, we search for the tag 'MainCamera'")]
        protected GameObject mainCamera;

        /// <summary>
        /// Determines how fast the character turns to face the movement direction.
        /// </summary>
        [SerializeField, Range(0.0f, 0.3f), Tooltip("How fast the character turns to face movement direction")]
        protected float rotationSmoothTime = 0.12f;

        private float _moveAngle;        // angle corresponding to the most recent player orientation input
        private float _rotationVelocity; // current angular speed at which the character turns
        private float _targetRotation;   // relative angle of motion

        /// <inheritdoc/>
        /// <summary>Direction of motion is always forward based the orientation of the character.</summary>
        protected override Vector3 Direction => Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

        protected override void Awake()
        {
            base.Awake();
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCamera == null) { Destroy(this); }
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Uses spherical coordinates formula to compute the target angle.
        /// <see href="https://en.wikipedia.org/wiki/Spherical_coordinate_system"/>
        /// </remarks>
        public override void Go(Vector2 move)
        {
            float phi = Mathf.Atan2(move.x, move.y);
            _moveAngle = phi * Mathf.Rad2Deg;

            TargetSpeed = IsSprinting ? sprintSpeed : walkSpeed;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Turns the character to face the direction of motion relative to the camera.
        /// </remarks>
        protected override void MoveBody(float speed)
        {
            OrientToCamera();
            base.MoveBody(speed);
        }

        /// <summary>
        /// Rotate character to face input direction relative to camera position.
        /// </summary>
        private void OrientToCamera()
        {
            _targetRotation = _moveAngle + mainCamera.transform.eulerAngles.y;

            Transform bodyTransform = transform;
            float rotation = Mathf.SmoothDampAngle(bodyTransform.eulerAngles.y, _targetRotation
                                                   , ref _rotationVelocity, rotationSmoothTime);

            bodyTransform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }
}
