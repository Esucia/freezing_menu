using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Jellies
{
    /// <summary>
    /// Currently a stub. Intended functionality:
    /// Handling of Jelly feeding mechanic, checking if the player can feed the Jelly.
    /// If the player feeds the Jelly, then this handles the Jelly's reactions and output objects
    /// </summary>
    public class Feeding : MonoBehaviour
    {

        private Parameters _parameters;

        private void Awake()
        {
            _parameters = GetComponent<Parameters>();
        }
        /// <summary>
        /// Feeds jelly by increasing it's food saturation.
        /// </summary>
        /// <param name="amountToIncrease">Amount of food to feed by, is same as saturation.</param>
        public void FeedJelly(float amountToIncrease)
        {
            _parameters.IncreaseFoodSaturation(amountToIncrease);
        }
    }
}


