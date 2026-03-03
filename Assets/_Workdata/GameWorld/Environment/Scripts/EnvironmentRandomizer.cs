using System;
using System.Collections;
using MyBox;
using UnityEngine;

public class EnvironmentRandomizer : MonoBehaviour
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    [Separator("Info")] [SerializeField] [ReadOnly]
    private string note = "Only change in DefaultSegment.";
#pragma warning restore CS0414 // Field is assigned but its value is never used

    [Separator("Transition Settings")]
    [SerializeField, PositiveValueOnly, Tooltip("Every x segments, the asset pool gets switched.")]
    private int segmentCountForTransition;

    [Separator("Asset Setup List")] [SerializeField] [Tooltip("Will be sorted as in the Array.")]
    private GameObject[] assetSetups;

    public int assetIndex;

    private GameObject currentAssetSegment;
    

    IEnumerator ExecuteAfterFrames(Action actionToExecute, float frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        if (actionToExecute != null)
        {
            actionToExecute();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < assetSetups.Length; i++)
        {
            assetSetups[i].SetActive(false);
        }
    }

    private void SpawnEnvironment()
    {
        if (assetSetups == null || assetSetups.Length == 0) return;

        if (assetIndex >= assetSetups.Length)
            assetIndex = 0;

        currentAssetSegment = assetSetups[assetIndex];
        currentAssetSegment.SetActive(true);
    }


    /// <summary>
    ///     Chooses the asset Pack based on the current asset Index.
    ///     Returns true when a transition has passed.
    /// </summary>
    public bool SetAssetSegment(int index, int currentSegmentCount)
    {
        assetIndex = index;

        if (assetSetups[assetIndex].TryGetComponent(out TransitionPack _))
            // _ is short for 'we dont need the value, we just want to know if it exists or not'
        {
            assetIndex++;
            SpawnEnvironment();
            return true;
        }

        if (currentSegmentCount >= segmentCountForTransition)
        {
            assetIndex++;
        }


        SpawnEnvironment();
        return false;
    }
}