using edeastudio.Attributes;
using edeastudio.Shared;
using UnityEngine;
using edeastudio.Utils;

namespace edeastudio.Components
{
    /// <summary>
    /// The audio SFX player.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]

    [eClassHeader("Audio SFX Player", iconName = "icon_v2", openClose = true)]
    public class AudioSFXPlayer : eMonoBehaviour
    {
        /// <summary>
        /// The audio list.
        /// </summary>
        [Space(10)]
        [SerializeField]
        private AudioFXList audioList;
        /// <summary>
        /// The instance.
        /// </summary>
        private static AudioSFXPlayer _instance;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static AudioSFXPlayer Instance { get => _instance; }

        /// <summary>
        /// The audio source.
        /// </summary>
        private AudioSource audioSource;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                _instance = null;
                return;
            }

            _instance = this;

            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
        }
        public void StopMusic()
        {
            if (audioSource)
            {
                audioSource.Stop();
            }
        }
        public void StartMusic()
        {
            if (audioSource && audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
        public void PlayOneShot(AudioClip clip)
        {
            if (clip != null) audioSource.PlayOneShot(clip);
        }

        public void PlayOneShot(string defaultName)
        {
            AudioClip clip = audioList.audioElements.Find(x => x.name == defaultName).clip;
            if (clip != null) audioSource.PlayOneShot(clip);
        }


        public AudioClip GetCurrentAudioClip()
        {
            return audioSource.clip ? audioSource.clip : null;
        }

        public void SetAudioClip(AudioClip clip)
        {
            audioSource.clip = clip;
            Play(clip, true);

        }
        public void Play(AudioClip clip, bool loop)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }
        public void Play(string defaultName, bool loop)
        {
            AudioClip clip = audioList.audioElements.Find(x => x.name == defaultName).clip;
            //Debug.Log(clip.name);
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }

    }

}