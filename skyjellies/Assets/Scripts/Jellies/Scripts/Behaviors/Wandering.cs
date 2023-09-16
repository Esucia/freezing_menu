using UnityEngine;
using UnityEngine.AI;

namespace Jellies.Behaviors
{
    /// <summary>
    /// Add this script to a jelly prefab to provide simple random movement via Unity's navmesh agent behavior.
    /// </summary>
    /// <remarks>
    /// Due to the limitations of Unity's <c>NavMeshAgent</c> behavior, this script currently implements jelly movement
    /// as a "slide" rather than the "hop" described by the design team. There are a few different ways we might
    /// implement a "hopping" movement:
    /// <list type="bullet">
    /// <item>Implement the "hop" visually via an <c>Animator</c> or transformations on a child object. I recommend
    /// one of these approaches and have provided rudimentary demonstrations of both via the <c>JellyHop</c> Animation
    /// clip and the <c>Hopping</c> MonoBehaviour script.</item>
    /// <item>Stop using <c>NavMesh</c> and <c>NavMeshAgent</c> and instead manually implement pathfinding and obstacle
    /// avoidance. Then we can make the jellies hop simply by applying impulses to their <c>Rigidbody</c>. This gives
    /// us maximum control but requires much more effort and is likely to be more error-prone.</item>
    /// <item>Instead of one <c>NavMesh</c> per island, give each block its own little <c>NavMesh</c> connected to
    /// other blocks by <c>NavMeshLink</c>s, and then use Unity's built-in support to hopping over the links. This
    /// likely will be much more resource intensive and may be too complicated to set up reliably.</item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Wandering : State
    {
        /// <summary>
        /// Register with this event if you need to know when the jelly changes its destination.
        /// </summary>
        public event System.Action<bool> Changed;

        /// <summary>
        /// Distance of the wander region center from the jelly.
        /// </summary>
        [SerializeField, Range(0, 5), Tooltip("Distance of the wander region center from the jelly.")]
        private float _wanderDistance = 0.5f;

        /// <summary>
        /// Radius of the wander region.
        /// </summary>
        /// <remarks>
        /// The jelly will never move backward if the wander range is less than the wander distance.
        /// </remarks>
        [SerializeField, Range(1, 10), Tooltip("Radius of the wander region.")]
        private float _wanderRange = 2.0f;

        /// <summary>
        /// Distance below which the jelly considers itself to be "arrived" at its destination.
        /// </summary>
        /// <remarks>
        /// We provide this as separate from the <c>NavMeshAgent</c> stopping distance since the value of the latter
        /// denotes the distance over which the agent decelerates as it arrives and may have little to do with how
        /// the concept of "nearness" that interests us here.
        /// </remarks>
        [SerializeField, Range(0, 1)
         , Tooltip("Distance below which the jelly considers itself to be arrived at its destination.")]
        private float _nearDistance = 0.1f;

        private NavMeshAgent _agent;
        private NavMeshPath  _currentPath;
        private bool _canWander = true;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _canWander = true;
        }

        private void Start()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.speed = 1f; // after changing the animation use this to fine tune the speed
            }
        }

        private void Update()
        {
            if (_canWander)
            {
                ChooseDestination();
            }
            else if (!_canWander)
            {
                if (_agent.remainingDistance > .1f + _agent.stoppingDistance)
                {
                    _agent.SetDestination(this.transform.position);
                }
            }
        }

        /// <summary>
        /// Determines whether the jelly is within some desired distance of its current destination.
        /// </summary>
        /// <param name="nearDistance">how far from the destination is considered "near"</param>
        /// <returns><c>true</c> if the jelly is within the desired distance, otherwise <c>false</c></returns>
        /// <remarks>
        /// This check returns <c>false</c> if the jelly does not currently have a valid, computed path.
        /// </remarks>
        private bool IsNearDestination(float nearDistance)
        {
            return !_agent.pathPending &&
                   _agent.remainingDistance <= Mathf.Min(_agent.stoppingDistance, nearDistance) &&
                   (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
        }

        /// <summary>
        /// Chooses a next destination randomly from a region centered in front of the jelly.
        /// </summary>
        /// <remarks>
        /// Currently, the jelly wanders by randomly choosing a next position from
        /// a spherical region centered in front of the jelly itself. Both the
        /// distance of the sphere center from the jelly and the radius of the
        /// sphere may be configured from the Inspector.
        /// </remarks>
        private bool ChooseDestination()
        {
            if (IsNearDestination(_nearDistance))
            {
                Transform jellyTransform = transform;
                Vector3 wanderCenter = jellyTransform.position + (jellyTransform.forward * _wanderDistance);
                if (!RandomPoint(wanderCenter, _wanderRange, out Vector3 point)) { return false; }

                _agent.stoppingDistance = 0.1f;
                _agent.SetDestination(point);
                bool near = 2 * Vector3.Distance(point, jellyTransform.position) < _wanderDistance + _wanderRange;
                Changed?.Invoke(near);
            }

            return true;
        }

        /// <summary>
        /// Attempts to compute a position on the agent's navmesh by
        /// projecting a randomly point from within a spherical region.
        /// </summary>
        /// <param name="center">the center of the spherical search region</param>
        /// <param name="range">the radius of the spherical search region</param>
        /// <param name="result">the projected point on the navmesh, if one was found</param>
        /// <returns><c>true</c> if it found a valid projection onto the navmesh, otherwise <c>false</c></returns>
        /// <remarks>
        /// Only projects a point if it's a maximum of twice the agent's height
        /// away from the navmesh, as recommended in Unity's documentation.
        /// \todo Consider alternatives or optimizations for this expensive method.
        /// </remarks>
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + (Random.insideUnitSphere * range);

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _agent.height * 2, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }

        public void ToggleWander(bool state)
        {
            _canWander = state;
        }
    }
}