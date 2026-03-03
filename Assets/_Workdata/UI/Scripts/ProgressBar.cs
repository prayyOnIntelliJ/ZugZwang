using System;
using System.Collections;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerAccountProgression))]
public class ProgressBar : MonoBehaviour
{
    [Separator("References")]
    [SerializeField, Tooltip("Only works when a maximum of 1 image is a child.")]
    private bool tryFindMask;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private string levelPrefix = "lvl. ";
    [SerializeField] private string xpSuffix = " XP";
    

    [SerializeField, ConditionalField(nameof(tryFindMask), true)]
    private Image mask;

    [SerializeField] private Animator rankUpAnimator;

    [Separator("Progress Settings")] [SerializeField]
    private bool fillAtEnable = false;
    [SerializeField, Range(1f, 10f), PositiveValueOnly, Tooltip("Time in seconds to fill the progress bar.")]
    private float fillTimeInSeconds = 2f;

    [SerializeField, Tooltip("Animation curve for the fill animation.")]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Separator("Rank Up Animation Triggers")]
    [SerializeField] private string rankUpBronzeTrigger = "PlayerLevelUpBronze";
    [SerializeField] private string rankUpSilverTrigger = "PlayerLevelUpSilver";
    [SerializeField] private string rankUpGoldTrigger = "PlayerLevelUpGold";

    [Separator("Runtime Data")]
    [ReadOnly] private PlayerAccountProgression playerAccountProgression;
    private int repeatCount;

    [Separator("Events")]
    public UnityEvent<int> OnRankUp;

    private void Awake()
    {
        playerAccountProgression = GetComponent<PlayerAccountProgression>();

        if (tryFindMask)
            mask = GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != gameObject) ?? mask;
    }

    public void StartFilling(int startLevelCount, int levelUpCount)
    {
        StartCoroutine(FillProgressBarLoop(startLevelCount, levelUpCount));
    }

    private IEnumerator FillProgressBarLoop(int startLevel, int levelUpCount)
    {
        int level = startLevel;
        int xpNeeded = playerAccountProgression.ranks[playerAccountProgression.GetCurrentRankIndex(level)].xpPerLevel;
        levelText.text = levelPrefix + level;

        float t0 = 0f;
        while (t0 < fillTimeInSeconds)
        {
            t0 += Time.deltaTime;
            float progress = t0 / fillTimeInSeconds;
            if (levelUpCount <= 1)
            {
                xpText.text =
                    Mathf.RoundToInt(Mathf.Lerp(0, playerAccountProgression.CurrentXp, curve.Evaluate(progress))) +
                    xpSuffix;
                mask.fillAmount = Mathf.Lerp(0, playerAccountProgression.GetLevelProgress(), curve.Evaluate(progress));
            }
            else
            {
                xpText.text =
                    Mathf.RoundToInt(Mathf.Lerp(playerAccountProgression.CurrentXp, xpNeeded,
                        curve.Evaluate(progress))) + xpSuffix;
                mask.fillAmount = Mathf.Lerp(playerAccountProgression.GetLevelProgress(), 1, curve.Evaluate(progress));
            }

            yield return null;
        }


        if (levelUpCount <= 0) yield break;
        level++;
        CheckRankUp(level - 1, level);

        levelText.text = levelPrefix + level;

        if (levelUpCount <= 1)
        {
            mask.fillAmount = playerAccountProgression.GetLevelProgress();
            yield break;
        }



        for (int i = 0; i < levelUpCount - 1; i++)
        {
            xpNeeded = playerAccountProgression.ranks[playerAccountProgression.GetCurrentRankIndex(level)].xpPerLevel;
            levelText.text = levelPrefix + level;

            float t = 0f;
            while (t < fillTimeInSeconds)
            {
                t += Time.deltaTime;
                float progress = t / fillTimeInSeconds;
                xpText.text = Mathf.RoundToInt(Mathf.Lerp(0, xpNeeded, curve.Evaluate(progress))) + xpSuffix;
                mask.fillAmount = Mathf.Lerp(0, 1, curve.Evaluate(progress));
                yield return null;
            }

            level++;
            CheckRankUp(level - 1, level);
        }

        float finalProgress = playerAccountProgression.GetLevelProgress();
        float t2 = 0f;

        levelText.text = levelPrefix + level;

        while (t2 < fillTimeInSeconds)
        {
            t2 += Time.deltaTime;
            float progress = t2 / fillTimeInSeconds;
            xpText.text =
                Mathf.RoundToInt(Mathf.Lerp(0, playerAccountProgression.CurrentXp, curve.Evaluate(progress))) + " XP";
            mask.fillAmount = Mathf.Lerp(0, finalProgress, curve.Evaluate(progress));
            yield return null;
        }

        mask.fillAmount = finalProgress;
    }
    
    public IEnumerator FillOnce()
    {
        float t = 0f;
        levelText.text = levelPrefix + playerAccountProgression.CurrentLevel;
        
        while (t < fillTimeInSeconds)
        {
            t += Time.deltaTime;
            float progress = t / fillTimeInSeconds;
            mask.fillAmount = Mathf.Lerp(0, playerAccountProgression.GetLevelProgress(), curve.Evaluate(progress));
            xpText.text =
                Mathf.RoundToInt(Mathf.Lerp(0, playerAccountProgression.CurrentXp, curve.Evaluate(progress))) +
                xpSuffix;
            yield return null;
        }
    }

    private void CheckRankUp(int levelBefore, int levelAfter)
    {
        int rankBefore = playerAccountProgression.GetCurrentRankIndex(levelBefore);
        int rankAfter = playerAccountProgression.GetCurrentRankIndex(levelAfter);
        if (rankAfter > rankBefore)
        {
            switch (playerAccountProgression.GetCurrentRankIndex(levelBefore))
            {
                case 0:
                    rankUpAnimator.SetTrigger(rankUpBronzeTrigger);
                    break;
                case 1:
                    rankUpAnimator.SetTrigger(rankUpSilverTrigger);
                    break;
                case 2:
                    rankUpAnimator.SetTrigger(rankUpGoldTrigger);
                    break;
            }

            OnRankUp?.Invoke(playerAccountProgression.GetCurrentRankIndex());
        }
    }
}