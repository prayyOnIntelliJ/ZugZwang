using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public enum ScreenTypes
    {
        MAIN_MENU = 0,
        HIGHSCORE_MENU = 10,
        SETTINGS_MENU = 20,
        CREDITS_MENU = 30
    }

    [Serializable]
    private struct ScreenEntry
    {
        public ScreenTypes type;
        public GameObject screen;
    }

    [Separator("Starting Options")]
    [SerializeField] private ScreenTypes startingType;

    [Separator("Screens")]
    [SerializeField] private List<ScreenEntry> screenEntries = new(4);

    [Separator("Animations")]
    [SerializeField] private Animator highscoreMessageAnimator;
    [SerializeField] private FadeAnimation fadeAnimation;

    private readonly Dictionary<ScreenTypes, GameObject> screens = new();
    private ScreenTypes currentScreen;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = GetComponent<SceneLoader>();
        
        if (!Prefs.HasKey(Prefs.KEY_TYPES.LEVEL)) Prefs.SetKey(Prefs.KEY_TYPES.LEVEL, 0);
        if (!Prefs.HasKey(Prefs.KEY_TYPES.XP)) Prefs.SetKey(Prefs.KEY_TYPES.XP, 0);

        foreach (var entry in screenEntries)
        {
            if (!screens.ContainsKey(entry.type))
                screens.Add(entry.type, entry.screen);
        }
    }

    private void Start()
    {
        foreach (var screen in screens.Values)
            screen.SetActive(false);

        if (screens.TryGetValue(startingType, out var startScreen))
            startScreen.SetActive(true);
        
        if (Prefs.GetKey<bool>(Prefs.KEY_TYPES.NEW_SCORE))
        {
            highscoreMessageAnimator.SetTrigger("NewHighscore");
            Prefs.SetKey(Prefs.KEY_TYPES.NEW_SCORE, false);
        }
    }

    public void SwitchToMainMenu() => SwitchScreen(ScreenTypes.MAIN_MENU);
    public void SwitchToHighscoreMenu() => SwitchScreen(ScreenTypes.HIGHSCORE_MENU);
    public void SwitchToSettingsMenu() => SwitchScreen(ScreenTypes.SETTINGS_MENU);
    public void SwitchToCreditsMenu() => SwitchScreen(ScreenTypes.CREDITS_MENU);

    private void SwitchScreen(ScreenTypes type)
    {
        foreach (var screen in screens.Values)
            screen.SetActive(false);

        if (screens.TryGetValue(type, out var targetScreen))
        {
            targetScreen.SetActive(true);
            currentScreen = type;
        }
        else
        {
            Debugger.LogWarning($"Screen type {type} not found in MainMenu.");
        }
    }

    public void StartGame()
    {
        if (Prefs.GetKey<bool>(Prefs.KEY_TYPES.FIRST_GAME))
        {
            sceneLoader.SetTargetScene("TutorialScene");
            fadeAnimation.StartFade();
        }
        else
        {
            sceneLoader.SetTargetScene("EndlessScene");
            fadeAnimation.StartFade();
        }
    }
}