using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Provides for basic player character physics and grounded state.
    /// </summary>
    /// <remarks>
    /// <p>This component is a minimal prerequisite for most other player-related components. The architecture we use
    /// in the <c>Player</c> namespace leans heavily on the Entity-Component pattern. We encapsulate each essential
    /// feature of the player mechanics in a dedicated component with single-responsibility (e.g., planar movement,
    /// jumping, camera orientation, animations, and sound effects are each governed by a small, specialized component).
    /// his architecture is event-driven in order to help limit coupling among the various components.</p>
    /// <p>This component is built atop and encapsulates Unity's <c>CharacterController</c>. Object or components that
    /// need to know when the player falls or becomes grounded again (e.g., an animator controller) should subscribe to
    /// the <c>fallen</c> and <c>landed</c> events.</p>
    /// \todo Determine whether to replace the Unity CharacterController with a custom implementation.
    /// </remarks>
    [RequireComponent(typeof(CharacterController))]
    public class Body : MonoBehaviour
    {
        /// <summary>
        /// Fired when player falls (i.e., is no longer grounded).
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when player starts falling after being grounded.
        /// </remarks>
        public event Action fallen;

        /// <summary>
        /// Fired when player lands (i.e., becomes grounded).
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when player becomes grounded again after falling.
        /// </remarks>
        public event Action landed;

        /// <summary>
        /// The character uses its own gravity value. The engine default is -9.81f.
        /// </summary>
        [SerializeField, Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        internal float gravity = -15.0f;

        /// <summary>
        /// Delay before entering the fall state (useful for walking down stairs).
        /// </summary>
        [SerializeField, Tooltip("Delay before entering the fall state. Useful for walking down stairs")]
        private float fallTimeout = 0.15f;

        /// <summary>
        /// Used to track whether character was in the "grounded" state in the most recent frame.
        /// </summary>
        /// <remarks>
        /// This is separate from the <c>CharacterController</c>'s own <c>grounded</c> state for two reasons:
        /// (a) to provide leeway in case of rough terrain, and (b) to allow for the fall timeout logic.
        /// </remarks>
        [Header("Player Grounded")]
        [SerializeField
         , Tooltip("Whether character was grounded in the current frame (separate from CharacterController grounded).")]
        internal bool grounded = true;

        /// <summary>
        /// Margin of error for detecting contact with the ground (useful for rough terrain).
        /// </summary>
        [SerializeField, Tooltip("Useful for rough ground")]
        private float groundedOffset = -0.14f;

        /// <summary>
        /// Defines which layers the player collides with for the purpose of determining grounded state.
        /// </summary>
        [SerializeField, Tooltip("What layers the character uses as ground")]
        private LayerMask _groundLayers;

        // various fields for book-keeping related to physics
        private const float _terminalVelocity = -53.0f;

        internal float verticalVelocity;
        private  float _timeUntilFall;

        private CharacterController _internalController;
        private Vector3             _velocity;

        /// <summary>
        /// Observer property for checking whether this character body is in motion.
        /// </summary>
        public bool IsMoving => _internalController.velocity == Vector3.zero;

        public Vector3 RespawnPosition => _respawnHere;

        internal Vector3 Force { get; set; }

        internal Vector3 Velocity => _internalController.velocity;

        //the player should respawn at this position if they fall out of the world
        private Vector3 _respawnHere;

        private void Awake()
        {
            _internalController = GetComponent<CharacterController>();
        }

        //check every few seconds and set the respawn position
        float checkCounter = 2.9f;
        private void Update()
        {
            CheckForGround();
            Move();

            checkCounter += Time.deltaTime;
            if (checkCounter > 3f)
            {
                checkCounter = 0;
                if (grounded)
                {
                    _respawnHere = transform.position;
                }
            }
        }

        /// <summary>
        /// Moves the character forward according to the current velocity and gravitational force.
        /// </summary>
        /// <remarks>
        /// Probably premature optimization, but since this method is on the hot path we attempt to avoid even unnecessary
        /// copies and/or stack allocations by modifying the target direction vector in-place.
        /// </remarks>
        private void Move()
        {
            Vector3 motion = Force;
            motion.y += ;
            motion   *= Time.deltaTime;
            _internalController.Move(motion);
        }

        /// <summary>
        /// Pulls the character downward incrementally, limited by a terminal velocity.
        /// </summary>
        private void ApplyGravity()
        {
            if ( > _terminalVelocity)
            {
                 += gravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// Character enters the "grounded" state and notifies interested observers.
        /// </summary>
        /// <remarks>
        /// When grounded, the character no longer falls and the fall-timeout is reset.
        /// </remarks>
        private void BecomeGrounded()
        {
            grounded       = true;
            _timeUntilFall = fallTimeout;
             = 0;
            landed?.Invoke();
        }

        /// <summary>
        /// Test for contact with ground and compare to current grounded state.
        /// </summary>
        private void CheckForGround()
        {
            Vector3 spherePosition = transform.position;
            spherePosition.y -= groundedOffset;

            bool touchingGround = Physics.CheckSphere(spherePosition, _internalController.radius
                                                      , _groundLayers, QueryTriggerInteraction.Ignore);

            if (!touchingGround)
            {
                if (grounded) { MaybeFall(); }

                ApplyGravity();
            }
            else if (!grounded) { BecomeGrounded(); }
        }

        /// <summary>
        /// Determines whether to enter the "falling" state and notifies interested observers if so.
        /// </summary>
        /// <remarks>
        /// When no longer touching the ground, the character is not yet considered to be "falling" until the fall
        /// timeout has elapsed. If the player touches the ground again before the timeout elapses, then
        /// </remarks>
        private void MaybeFall()
        {
            if (_timeUntilFall > 0)
            {
                _timeUntilFall -= Time.deltaTime;
                return;
            }

            grounded = false;
            fallen?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed   = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Vector3 position = transform.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - groundedOffset, position.z)
                              , 0.28f); // ideally get radius from the internal CharacterController
        }
    }
}
