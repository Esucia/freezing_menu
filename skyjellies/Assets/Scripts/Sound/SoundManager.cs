using UnityEngine;
using AK.Wwise;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField]
        private RTPC _masterVolume;
        [SerializeField]
        private RTPC _musicVolume;
        [SerializeField]
        private RTPC _sfxVolume;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            } else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Set master bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetMasterVolume(float value)
        {
            SetGlobalSoundParameter(_masterVolume, value);
        }

        /// <summary>
        /// Set music bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetMusicVolume(float value)
        {
            SetGlobalSoundParameter(_musicVolume, value);
        }

        /// <summary>
        /// Set sfx bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetSFXVolume(float value)
        {
            SetGlobalSoundParameter(_sfxVolume, value);
        }

        /// <summary>
        /// Set global Wwise game parameter value.
        /// </summary>
        /// <param name="parameter">AK.Wwise.RTPS parameter to change</param>
        /// <param name="value"></param>
        private void SetGlobalSoundParameter(RTPC parameter, float value)
        {
            parameter.SetGlobalValue(value);
        }
    }

}