using System.Collections;
using Jellies.Behaviors;
using UnityEngine;

namespace Jellies
{
    /// <summary>
    /// Add this script to a jelly prefab to demonstrates a basic hopping animation applied to a visible child object.
    /// </summary>
    /// <remarks>
    /// We provide this solely as a demonstration and proof-of-concept to convey the designers' idea of a hopping jelly.
    /// </remarks>
    [RequireComponent(typeof(Wandering))]
    public class Hopping : MonoBehaviour
    {
        /// <summary>
        /// Reference to a child object representing the jelly's visible form.
        /// </summary>
        /// <remarks>
        /// This game object must be a child of the game object bearing this component so that it inherits the parent
        /// transform, otherwise it will not follow the movement of the jelly agent. It should also be a visible shape
        /// so that the effect of the hopping animation can be seen.
        /// </remarks>
        [SerializeField, Tooltip("Reference to a child object representing the jelly's visible form.")]
        private GameObject _jellyShape;

        /// <summary>
        /// How long (in seconds) the jelly is in the air during a single hop.
        /// </summary>
        [SerializeField, Tooltip("How long (in seconds) the jelly is in the air during a single hop.")]
        private float _hopDuration;

        /// <summary>
        /// How high the jelly hops relative to its own height.
        /// </summary>
        [SerializeField, Tooltip("How high the jelly hops relative to its own height.")]
        private float _hopHeight;

        private IEnumerator _hopRoutine;
        private bool        _isHopping;
        private bool        _isStopped = true;

        private void Awake()
        {
            Wandering wandering = GetComponent<Wandering>();
            wandering.Changed += OnChanged;
            wandering.Exited += OnStopped;
        }

        private void Update()
        {
            if (_isStopped || _isHopping) { return; }

            StartCoroutine(Hop());
        }

        private void OnChanged(bool isNear)
        {
            _isStopped = isNear;
        }

        private void OnStopped(State state)
        {
            _isStopped = true;
        }

        /// <summary>
        /// Coroutine to demonstrate a simple hopping motion for the jelly's placeholder cube shape.
        /// </summary>
        /// <returns>permits iterating over frames to animate the vertical position of the jelly shape</returns>
        /// <remarks>Implements a hop as a simple, non-physical parabolic path.</remarks>
        private IEnumerator Hop()
        {
            _isHopping = true;
            Transform shapeTransform = _jellyShape.transform;

            float q = _hopHeight / (_hopDuration * _hopDuration);
            for (float x = 0; x < _hopDuration; x += Time.deltaTime)
            {
                shapeTransform.localPosition = new Vector3(0, q * x * (_hopDuration - x), 0);
                yield return null;
            }

            _isHopping = false;
        }
    }
}
