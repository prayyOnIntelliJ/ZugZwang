using MyBox;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsOverwriter : MonoBehaviour
{
    [Separator("Graphic Settings")]
    [SerializeField] private int defaultGraphicIndex = 2;
    [SerializeField] private float defaultDB = -6f;
    [SerializeField] private GraphicsSO graphicsSO;
    [Separator("Sound Settings")]
    [SerializeField] private AudioMixerGroup soundGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    private void Start()
    {
        int graphicIndex;
        
        if (Prefs.ExistsOrCreateKey(Prefs.KEY_TYPES.GRAPHICS, defaultGraphicIndex))
        {
            graphicIndex = Prefs.GetKey<int>(Prefs.KEY_TYPES.GRAPHICS);
        }
        else
        {
            graphicIndex = defaultGraphicIndex;
        }
        
        SetGraphicSettings(graphicIndex);
        SetSoundSettings();
    }

    public void SetGraphicSettings(int currentGraphicIndex)
    {
        graphicsSO.SetGraphicSettings(currentGraphicIndex);
    }

    private void SetSoundSettings()
    {
        // DB -> Linear: Mathf.Pow(10, db / 20f)
        // Linear -> DB: Mathf.Log10(value) * 20
        AudioMixer audioMixer = soundGroup.audioMixer ? soundGroup.audioMixer : musicGroup.audioMixer;

        if (audioMixer == null) return;
        
        float musicDB = Prefs.HasKey(Prefs.KEY_TYPES.MUSIC) ? Prefs.GetKey<float>(Prefs.KEY_TYPES.MUSIC) : defaultDB;
        float soundDB = Prefs.HasKey(Prefs.KEY_TYPES.SOUND) ? Prefs.GetKey<float>(Prefs.KEY_TYPES.SOUND) : defaultDB;
        
        audioMixer.SetFloat("Music", musicDB);
        audioMixer.SetFloat("SFX", soundDB);
        
    }
}
