using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(SoundSliderReferences))]
public class SoundSlider : Slider
{
    private SoundSliderReferences sliderReferences;
    private AudioMixer audioMixer;
    private AudioMixerGroup soundGroup;

    private new void Awake()
    {
        sliderReferences = GetComponent<SoundSliderReferences>();
        soundGroup = sliderReferences.soundGroup;
        audioMixer = soundGroup.audioMixer;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        float defaultVolume = 0.5f;
        float musicVolume = Prefs.HasKey(Prefs.KEY_TYPES.SOUND) ? Mathf.Pow(10, Prefs.GetKey<float>(Prefs.KEY_TYPES.SOUND) / 20f) : defaultVolume;
        value = musicVolume;
    }

    public void SetSoundVolume()
    {
        float volume = value;
        float newVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(soundGroup.name, newVolume);

        Prefs.SetKey(Prefs.KEY_TYPES.SOUND, newVolume);
    }
}
