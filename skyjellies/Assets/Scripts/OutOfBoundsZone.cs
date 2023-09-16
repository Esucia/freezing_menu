using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Teleports the player back to solid ground if they fall out of the world.
    /// </summary> 
    /// 
    public class OutOfBoundsZone : MonoBehaviour
    {
        Transform _playerTransform;
        bool _shouldMovePlayer = false;
        bool _turnThePlayerBackOn = false;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _playerTransform = collision.gameObject.transform;
                _shouldMovePlayer = true;
            }
        }

        private void LateUpdate()
        {
            //this is so the position setting isn't overridden by the character controller
            if (_turnThePlayerBackOn)
            {
                _playerTransform.gameObject.GetComponent<Motion.RectilinearMovement>().enabled = true;
                _playerTransform.gameObject.GetComponent<Motion.Jumping>().enabled = true;
                _playerTransform.gameObject.GetComponent<Body>().enabled = true;
                _playerTransform.gameObject.GetComponent<CharacterController>().enabled = true;
                _turnThePlayerBackOn = false;
            }
            else if (_shouldMovePlayer)
            {
                _playerTransform.position = _playerTransform.gameObject.GetComponent<Body>().RespawnPosition;
                _shouldMovePlayer = false;
                _turnThePlayerBackOn = true;

                _playerTransform.gameObject.GetComponent<Motion.RectilinearMovement>().enabled = false;
                _playerTransform.gameObject.GetComponent<Motion.Jumping>().enabled = false;
                _playerTransform.gameObject.GetComponent<Body>().enabled = false;
                _playerTransform.gameObject.GetComponent<CharacterController>().enabled = false;
            }
        }
    }
}
