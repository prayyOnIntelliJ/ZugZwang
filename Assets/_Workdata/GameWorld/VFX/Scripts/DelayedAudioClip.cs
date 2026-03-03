using System.Collections;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DelayedAudioClip : MonoBehaviour
{
    [Separator("Audio Settings")]
    [SerializeField, OverrideLabel("Delay In Seconds")] private float delay = 0.5f;
    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(DelayAudioClip());
    }

    private IEnumerator DelayAudioClip()
    {
        yield return new WaitForSeconds(delay);
        source.Play();
    }
}
