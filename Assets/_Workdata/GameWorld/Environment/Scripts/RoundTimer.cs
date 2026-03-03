using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

/// <summary> Used to define round properties</summary>
[Serializable]
public class PhaseSettings
{
    public int segmentCount;
    public float moveTime;
    public float thinkTime;
}

public class CurrentPhaseProperties
{
    public float currentRoundLength;
    public int currentSegmentCount;
    public int settingActiveCount;
    public float moveTime;
    public float thinkTime;
}

/// <summary> Used for around by the game</summary>
public class RoundTimer : MonoBehaviour
{
    [Separator("Phase Settings")] [SerializeField]
    private List<PhaseSettings> phaseSettings;

    [Separator("Time Management")] [SerializeField]
    private TextMeshProUGUI timerTextField;

    [SerializeField] private float roundEndTime = 0.0f;
    [SerializeField] private float earlyRoundEndTime = 0.1f;

    [Separator("Debugging")] [SerializeField]
    private TextMeshProUGUI scaleFieldText;

    [SerializeField] private TextMeshProUGUI scaleText;

    [Separator("Runtime")] [SerializeField]  int currentFieldCount;
    private float currentRoundLength;

    public Action<CurrentPhaseProperties> OnTurn;
    public Action OnEarlyTurn;
    private bool hasBeenActivated = false;
    public int currentCount { private set; get; }

    private void Start()
    {
        ResetTimer();
        currentCount = 0;
    }

    private void Update()
    {
        currentRoundLength -= Time.deltaTime;

        if (currentRoundLength <= roundEndTime)
        {
            ResetTimer();
            OnTurn?.Invoke(GetSegmentInformation(GetCurrentPhaseSettings()));
            currentFieldCount++;
            hasBeenActivated = false;
        }

        if (currentRoundLength <= earlyRoundEndTime && !hasBeenActivated)
        {
            OnEarlyTurn?.Invoke();
            hasBeenActivated = true;
        }

        if (timerTextField)
            timerTextField.text = MathF.Round(currentRoundLength, 1).ToString("F1");

        CheckForRoundScaling();
    }

    private void ResetTimer()
    {
        currentRoundLength = GetCurrentRoundLength();
    }

    private CurrentPhaseProperties GetSegmentInformation(PhaseSettings currentSettings)
    {
        CurrentPhaseProperties phaseProperties = new CurrentPhaseProperties();
        phaseProperties.currentRoundLength = currentRoundLength;
        phaseProperties.currentSegmentCount = currentFieldCount;
        phaseProperties.settingActiveCount = phaseSettings.Count;
        phaseProperties.moveTime = currentSettings.moveTime;
        phaseProperties.thinkTime = currentSettings.thinkTime;


        return phaseProperties;
    }

    public CurrentPhaseProperties GetCurrentPhaseInformation()
    {
        return GetSegmentInformation(phaseSettings[currentCount]);
    }

    private void CheckForRoundScaling()
    {
        if (currentFieldCount > phaseSettings[currentCount].segmentCount &&
            currentCount < phaseSettings.Count - 1) currentCount++;

        if (scaleText != null)
            scaleText.text = GetCurrentRoundLength().ToString();
    }

    public float GetCurrentRoundLength()
    {
        return phaseSettings[currentCount].moveTime + phaseSettings[currentCount].thinkTime;
    }

    public PhaseSettings GetCurrentPhaseSettings()
    {
        return phaseSettings[currentCount];
    }
}