using UnityEngine;
using UnityEngine.Serialization;

namespace Jellies
{
    /// <summary>
    /// Add this script to an object in the jelly prefab hierarchy to provide for basic sensing of its environment.
    /// </summary>
    /// <remarks>
    /// /todo Add detailed notes here
    /// </remarks>
    [RequireComponent(typeof(SphereCollider))]
    public class Sensing : MonoBehaviour
    {
        /// <summary>
        /// Dispatched to registered listeners when the jelly senses something.
        /// </summary>
        internal event System.Action<EventArgs> Sensed;

        [FormerlySerializedAs("_senseRangeOverride")]
        [SerializeField, Range(0, 100), Tooltip("Positive values override the collider radius")]
        private float _sensorRangeOverride;

        private void Start()
        {
            if (_sensorRangeOverride > 0)
            {
                GetComponent<SphereCollider>().radius = _sensorRangeOverride;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Sensed?.Invoke(new(Sensor.Other, other.gameObject));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Sensed?.Invoke(new(Sensor.Other, other.gameObject, false));
            }
        }

        public enum Sensor { Warmth, Food, Other }

        /// <summary>
        /// Inner class representing event data describing what the jelly sensed.
        /// </summary>
        public class EventArgs : System.EventArgs
        {
            /// <summary>
            /// Movement component that generated the event.
            /// </summary>
            public readonly Sensor stimulus;

            /// <summary>
            /// Movement component that generated the event.
            /// </summary>
            public readonly GameObject source;

            /// <summary>
            /// Indicates that a previously sensed object is no longer sensed.
            /// </summary>
            public readonly bool status;

            /// <summary>
            /// Provides information about a sensory event.
            /// </summary>
            /// <param name="stimulus">indicates the type of sensory data</param>
            /// <param name="source">game object that caused this sensory change</param>
            /// <param name="status">whether a sensory input is present or not</param>
            internal EventArgs(Sensor stimulus, GameObject source, bool status = true)
            {
                this.stimulus = stimulus;
                this.source   = source;
                this.status   = status;
            }
        }
    }
}
