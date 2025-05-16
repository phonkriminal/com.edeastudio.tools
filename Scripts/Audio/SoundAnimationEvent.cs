using System.Collections;
using UnityEngine;


public class SoundAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSourcePrefab;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float _volume;

    [SerializeField]
    [Range(-2f, 2f)]
    private float _pitch;

    [SerializeField]
    [Range(0, 1)]
    private float _spatialBlend;

    [SerializeField]
    [Range(0, 2)]
    private float _timeToTrigger;

    private float audioLenght;

    private Coroutine cTimeToTrigger;

    private void Start()
    {
        audioLenght = audioClip.length;
    }


    private void InitAudio()
    {
        audioSource = Instantiate(audioSourcePrefab);
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.pitch = _pitch;
        audioSource.spatialBlend = _spatialBlend;
        audioSource.volume = _volume;
        audioSource.clip = audioClip;
    }

    public void PlaySound()
    {
        cTimeToTrigger = StartCoroutine(TriggerSound(_timeToTrigger));
    }

    private IEnumerator TriggerSound(float timeTotrigger)
    {
        InitAudio();

        yield return new WaitForSeconds(timeTotrigger);

        audioSource.Play();
        Destroy(audioSource.gameObject, audioLenght);
    }
}
