using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Jellies
{
    /// <summary>
    /// The handling of updating and storing the main information about a Jelly.
    /// </summary>
    public class Parameters : MonoBehaviour
    {
        public enum JellyType { Base, Grass }

        [SerializeField]
        JellyType _typeOfJelly;

        public JellyType TypeOfThisJelly()
        {
            return _typeOfJelly;
        }
 

        [Header("Hunger")]
        [Tooltip("Threshold at which jelly becomes hungry.")]
        [SerializeField] private float _hungerThreshold = 20f;
        [Tooltip("Time interval it takes for saturation to decrease.")]
        [SerializeField] private float _saturationDecreaseTime = 10f;
        [Tooltip("How much saturation is lost each interval.")] 
        [SerializeField] private float _saturationDecreaseValue = 3f;
        
        /// <summary>
        /// How well fed jelly is.
        /// Decreases over time every interval provided in <cref>_saturationDecreaseTime</cref>.
        /// </summary>
        public float FoodSaturation { get; private set; } = 100f;
        /// <summary>
        /// Maximum amount of saturation that jelly can have.
        /// </summary>
        [field:SerializeField] 
        public float MaxFoodSaturation { get; private set; } = 100f;
        
        void Start()
        {
            FoodSaturation = MaxFoodSaturation;
            
            StartCoroutine(DecreaseFoodSaturationRoutine());
        }
        /// <summary>
        /// Increases jelly food saturation.
        /// </summary>
        /// <param name="saturationToAdd">Amount of saturation to increase.</param>
        public void IncreaseFoodSaturation(float saturationToAdd)
        {
            FoodSaturation = Mathf.Clamp(FoodSaturation + saturationToAdd, 0, MaxFoodSaturation);
            
            print($"Jelly is fed by 1 Red Berry = {saturationToAdd} saturation\nFood Saturation: {FoodSaturation}");
        }
        /// <summary>
        /// used for unit testing can be set to private.
        /// </summary>
        /// <param name="saturationToRemove"></param>
        public void DecreaseFoodSaturation(float saturationToRemove)
        {
            FoodSaturation = Mathf.Clamp(FoodSaturation - saturationToRemove, 0, MaxFoodSaturation);
            CheckIfJellyIsHungry();
        }
        /// <summary>
        /// Used for unit testing can be removed.
        /// </summary>
        /// <param name="saturationValue"></param>
        public void SetFoodSaturation(float saturationValue)
        {
            FoodSaturation = Mathf.Clamp(saturationValue, 0, MaxFoodSaturation);
        }

        private void CheckIfJellyIsHungry()
        {
            if (FoodSaturation == 0)
            {
                //Debug.Log("What happens when jelly has 0 food ?");
            }
            if (FoodSaturation <= _hungerThreshold)
            {
                //Debug.Log("Play Jelly hungry sound"); //TODO: Add hungry sound
            }
        }

        private IEnumerator DecreaseFoodSaturationRoutine()
        {
            yield return new WaitForSeconds(_saturationDecreaseTime);
            DecreaseFoodSaturation(_saturationDecreaseValue);
            StartCoroutine(DecreaseFoodSaturationRoutine());
        }
    }
}
