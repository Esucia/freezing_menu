using UnityEngine;
using UnityEngine.AI;

namespace Jellies.Behaviors
{
    /// <summary>
    /// Add this script to a jelly prefab to provide curiosity where the jelly stops and observers a stranger.
    /// </summary>
    /// <remarks>
    /// Requires the jelly to have Unity's <c>NavMeshAgent</c> component.
    /// \todo Provide some more context and explanation here.
    /// </remarks>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Watching : State
    {
        /// <summary>
        /// Distance from which the jelly observes, as percentage of discovery distance.
        /// </summary>
        [SerializeField, Range(0, 10)
         , Tooltip("Distance from which the jelly observes, as percentage of discovery distance")]
        private float _watchDistance = 4f;

        /// <summary>
        /// Reference to a <c>NavMeshAgent</c> on the same GameObject as this component.
        /// </summary>
        private NavMeshAgent _agent;

        /// <summary>
        /// Current subject of the jelly's curiosity; e.g., a stranger such as the player.
        /// </summary>
        private GameObject Subject { get; set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            Watch();
        }

        /// <summary>
        /// Jelly will move toward and face the subject of its curiosity.
        /// </summary>
        /// <returns><c>true</c> if a path to the subject is valid, false otherwise</returns>
        private bool Watch()
        {
            Vector3 target = Subject.transform.position;
            if (_agent.destination == target) { return true; }

            _agent.stoppingDistance = _watchDistance;
            return _agent.SetDestination(target);
        }

        /// <remarks>
        /// Sets the subject of the jelly's curiosity before entering this state.
        /// </remarks>
        public override void Enter(GameObject entity)
        {
            Subject = entity;
            base.Enter(entity);
        }

        /// <remarks>
        /// Cancels the current navmesh agent path before exiting this state. 
        /// </remarks>
        public override void Exit()
        {
            _agent.ResetPath();
            base.Exit();
        }
    }
}