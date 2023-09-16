using Player.Motion;
using UnityEngine;

namespace Player.Equipment
{
    /// <summary>
    /// Demonstrates a single-use "Trainers" item that, when obtained, bestows the <c>Jumping</c> mechanic.
    /// </summary>
    /// <remarks>The GameObject hosting this component must also have a RigidBody and a trigger Collider.</remarks>
    [RequireComponent(typeof(Rigidbody))]
    public class Trainers : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Jumping jumping))
            {
                jumping.hasTrainers = true;
                Destroy(gameObject);
            }
        }
    }
}
