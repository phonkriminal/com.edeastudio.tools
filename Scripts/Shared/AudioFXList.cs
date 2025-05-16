using System.Collections.Generic;
using UnityEngine;

namespace edeastudio.Shared
{

    [CreateAssetMenu(fileName = "NewFXAudioList", menuName = "edeaStudio/FX Audio List", order = 0)]
    public class AudioFXList : ScriptableObject
    {
        public List<AudioElement> audioElements = new List<AudioElement>();

    }

    [System.Serializable]
    public class AudioElement
    {

        public AudioCategory category;

        public string name;

        public AudioClip clip;

    }


    [System.Serializable]
    public enum AudioCategory
    {
        UI, FX, Music, Other
    }

}