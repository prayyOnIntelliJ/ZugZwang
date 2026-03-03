using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PlayerAccountProgression : MonoBehaviour
{
    private const string PLAYERPREFS_LEVEL_KEY = "level";
    private const string PLAYERPREFS_XP_KEY = "xp";

    [Separator("Rank Settings")] [SerializeField]
    private RanksSO rankSO;

    [HideInInspector] public List<RanksSO.Rank> ranks;
    private int levelUpCount;

    private ProgressBar progressBar;

    public int CurrentLevel
    {
        get => Prefs.GetKey<int>(Prefs.KEY_TYPES.LEVEL);
        set => Prefs.SetKey(Prefs.KEY_TYPES.LEVEL, value);
    }

    public int CurrentXP
    {
        get => Prefs.GetKey<int>(Prefs.KEY_TYPES.XP);
        set => Prefs.SetKey(Prefs.KEY_TYPES.XP, value);
    }

    private void Awake()
    {
        ranks = rankSO.ranks;
        progressBar = GetComponent<ProgressBar>();
    }

    private void Start()
    {
        SetDefaultValues();
    }

    public float GetLevelProgress()
    {
        var currentRankIndex = GetCurrentRankIndex(CurrentLevel);
        float xpGained = ranks[currentRankIndex].xpPerLevel;
        return CurrentXP / xpGained;
    }


    public int GetLevelUpCount()
    {
        return levelUpCount;
    }

    public int GetCurrentRankIndex()
    {
        return GetCurrentRankIndex(CurrentLevel);
    }

    public int GetCurrentRankIndex(int level)
    {
        if (ranks == null || ranks.Count == 0)
            return -1;

        if (level < ranks[0].levelToReach)
            return 0;

        for (var i = 0; i < ranks.Count - 1; i++)
            if (level >= ranks[i].levelToReach &&
                level < ranks[i + 1].levelToReach)
                return i;

        return ranks.Count - 1;
    }


    public void AddXP(int amount)
    {
        var currentXp = CurrentXP;
        var currentLevel = CurrentLevel;

        currentXp += amount;

        CheckForLevelup(currentXp, currentLevel);
    }

    private void SetDefaultValues()
    {
        if (!Prefs.HasKey(Prefs.KEY_TYPES.LEVEL)) Prefs.SetKey(Prefs.KEY_TYPES.LEVEL, 0);
        if (!Prefs.HasKey(Prefs.KEY_TYPES.XP)) Prefs.SetKey(Prefs.KEY_TYPES.XP, 0);
    }

    private void CheckForLevelup(int currentXp, int currentLevel)
    {
        while (true)
        {
            var currentRankIndex = GetCurrentRankIndex(currentLevel);

            if (currentRankIndex >= ranks.Count)
                break;

            var xpNeeded = ranks[currentRankIndex].xpPerLevel;

            if (currentXp >= xpNeeded)
            {
                currentXp -= xpNeeded;
                currentLevel++;
                levelUpCount++;
            }
            else
            {
                break;
            }
        }

        CurrentXP = currentXp;
        CurrentLevel = currentLevel;
        PlayerPrefs.Save();

        var startingCount = currentLevel - GetLevelUpCount();
        progressBar.StartFilling(startingCount, levelUpCount);
    }
}