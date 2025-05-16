using System.Collections;
using System.Collections.Generic;
using edeastudio.Attributes;
using UnityEngine;
using edeastudio.Utils;

namespace edeastudio.Components
{
    /// <summary>
    /// The sound looping.
    /// </summary>
    [AddComponentMenu("edeaStudio/Sound Looping")]

    [eClassHeader("Sound Looping", iconName = "icon_v2")]
    public class SoundLooping : eMonoBehaviour
    {
        /// <summary>
        /// The audio source.
        /// </summary>
        [eEditorToolbar("Audio Component")]
        [Space(5)]
        [SerializeField]
        private AudioSource audioSource;
        /// <summary>
        /// The audio source prefab.
        /// </summary>
        [SerializeField]
        private GameObject audioSourcePrefab;
        /// <summary>
        /// The audio source object.
        /// </summary>
        private GameObject audioSourceObject;

        /// <summary>
        /// The audio clips.
        /// </summary>
        [eEditorToolbar("Audio Clips")]
        [Space(10)]
        [SerializeField]
        private List<AudioClip> audioClips;
        /// <summary>
        /// Start time.
        /// </summary>
        [SerializeField]
        private List<float> startTime;

        /// <summary>
        /// The volume.
        /// </summary>
        [eEditorToolbar("Audio Source")]
        [Space(10)]

        [Range(0.1f, 1f)]
        [SerializeField]
        private float _volume = 0.5f;

        /// <summary>
        /// The pitch.
        /// </summary>
        [SerializeField]
        [Range(-2f, 2f)]
        private float _pitch = 1f;

        /// <summary>
        /// The spatial blend.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        private float _spatialBlend = 0.5f;

        /// <summary>
        /// Time to trigger.
        /// </summary>
        [SerializeField]
        [Range(0f, 5f)]
        private float _timeToTrigger;

        /// <summary>
        /// The audio lenght.
        /// </summary>
        private float audioLenght;

        /// <summary>
        /// Play loop.
        /// </summary>
        private Coroutine PlayLoop;

        /// <summary>
        /// Repeat.
        /// </summary>
        private bool repeat = false;

        private void Start()
        {
            for (int i = 0; i < audioClips.Count; i++)
            {
                audioLenght += audioClips[i].length;
            }
            repeat = false;

            if (!audioSource && !audioSourcePrefab)
            {
                gameObject.AddComponent<AudioSource>();
                audioSource = GetComponent<AudioSource>();
            }
            else if (!audioSource && audioSourcePrefab)
            {
                audioSourceObject = Instantiate(audioSourcePrefab);
                audioSource = audioSourceObject.GetComponent<AudioSource>();
            }

            if (audioSource) Invoke("StartLoop", _timeToTrigger);
        }
        private void InitAudio()
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.pitch = _pitch;
            audioSource.spatialBlend = _spatialBlend;
            repeat = false;
        }

        private void StartLoop()
        {
            PlayLoop = StartCoroutine(PlayLoopChain());
        }
        private IEnumerator PlayLoopChain()
        {
            repeat = false;

            for (int i = 0; i < audioClips.Count; i++)
            {
                InitAudio();
                float timeToDestroy = audioClips[i].length;
                yield return new WaitForSeconds(startTime[i]);
                audioSource.PlayOneShot(audioClips[i], _volume);
            }

            repeat = true;
        }

        private void Update()
        {
            if (repeat)
            {
                PlayLoop = StartCoroutine(PlayLoopChain());
            }

        }

    }
}

/*#if UNITY_EDITOR
namespace edeastudio.Components
{
    [CustomEditor(typeof(SoundLooping))]
    public class SoundLoopingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUISkin skin = Resources.Load("eSkin") as GUISkin;
            GUI.skin = skin;
            
        }
    } 
}
#endif*/