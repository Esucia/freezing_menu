using Player.Motion;
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// Controls transitions between animation states in response to player character events.
    /// </summary>
    /// <remarks>
    /// <p>We animate the character's avatar in response to events dispatched from other <c>Player</c> components. This
    /// means that the avatar is loosely coupled to these components through their public event delegates.</p>
    /// <p>While this avatar controller requires the <c>Body</c>, <c>Movement</c>, and <c>Jumping</c> components in
    /// order to drive the animator, it is also designed to gracefully handle the absence of these components.</p>
    /// \todo Maybe loosen coupling further by extracting interfaces or adding a centralized player event dispatcher.
    /// </remarks>
    [RequireComponent(typeof(Animator))]
    public class AvatarController : MonoBehaviour
    {
        private Animator _animator;
        private float    _animationBlend;
        private int      _animIDSpeed;
        private int      _animIDGrounded;
        private int      _animIDJump;
        private int      _animIDFreeFall;
        private int      _animIDMotionSpeed;

        private void Awake()
        {
            bool hasAnimator = TryGetComponent(out _animator);
            if (!hasAnimator)
            {
                Destroy(this);
                return;
            }

            AssignAnimationIDs();
        }

        private void OnDisable()
        {
            UnregisterEventHandlers();
        }

        private void OnEnable()
        {
            RegisterEventHandlers();
        }

        /// <summary>
        /// Initiate a free-fall animation in response to a <c>Fallen</c> event fired by the <c>Body</c> component.
        /// </summary>
        private void OnFall()
        {
            _animator.SetBool(_animIDJump,     false);
            _animator.SetBool(_animIDGrounded, false);
            _animator.SetBool(_animIDFreeFall, true);
        }

        /// <summary>
        /// Initiate a jump animation in response to a <c>Jumped</c> event fired by the <c>Jumping</c> component.
        /// </summary>
        private void OnJump()
        {
            _animator.SetBool(_animIDFreeFall, false);
            _animator.SetBool(_animIDGrounded, false);
            _animator.SetBool(_animIDJump,     true);
        }

        /// <summary>
        /// Initiate a grounded animation in response to a <c>Landed</c> event fired by the <c>Body</c> component.
        /// </summary>
        private void OnLand()
        {
            _animator.SetBool(_animIDJump,     false);
            _animator.SetBool(_animIDFreeFall, false);
            _animator.SetBool(_animIDGrounded, true);
        }

        /// <summary>
        /// Update walking motion animation in response to a <c>Moved</c> event fired by the <c>Movement</c> component.
        /// </summary>
        /// <param name="args">event data representing the current motion of the character physics body</param>
        private void OnMove(Movement.EventArgs args)
        {
            _animationBlend = Mathf.Lerp(_animationBlend, args.TargetSpeed, Time.deltaTime * args.ChangeRate);
            if (_animationBlend < 0.01f) { _animationBlend = 0f; }
            _animator.SetFloat(_animIDSpeed,       _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, args.Magnitude);
        }

        /// <summary>
        /// Acquire and cache the unique IDs for the required animator parameters.
        /// </summary>
        private void AssignAnimationIDs()
        {
            _animIDSpeed       = Animator.StringToHash("Speed");
            _animIDGrounded    = Animator.StringToHash("Grounded");
            _animIDJump        = Animator.StringToHash("Jump");
            _animIDFreeFall    = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        /// <summary>
        /// Attempts to register as a listener for events dispatched from other <c>Player</c> components.
        /// </summary>
        /// <remarks>
        /// Gracefully fails if unable to find the required components (currently <c>Body</c>, <c>Movement</c>, and
        /// <c>Jumping</c>), in which case the avatar will not animate.
        /// </remarks>
        private void RegisterEventHandlers()
        {
            if (TryGetComponent(out Body body))
            {
                body.Fallen += OnFall;
                body.Landed += OnLand;
            }

            if (TryGetComponent(out Movement motion))
            {
                motion.Moved += OnMove;
            }

            if (TryGetComponent(out Jumping trainers))
            {
                trainers.Jumped += OnJump;
            }
        }

        /// <summary>
        /// Attempts to remove this component from the invocation list for Player events.
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (TryGetComponent(out Body body))
            {
                body.Fallen -= OnFall;
                body.Landed -= OnLand;
            }

            if (TryGetComponent(out Movement motion))
            {
                motion.Moved -= OnMove;
            }

            if (TryGetComponent(out Jumping trainers))
            {
                trainers.Jumped -= OnJump;
            }
        }
    }
}
