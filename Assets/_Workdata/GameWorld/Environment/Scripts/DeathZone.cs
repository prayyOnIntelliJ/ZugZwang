using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class DeathZone : MonoBehaviour
{
    [Separator("References")] [SerializeField]
    private HighScore highScore;
    
    [SerializeField, Tag] string playerTag = "Player";

    [SerializeField] private FigureFactory figureFactory;

    [HideInInspector] public int currentPlayerCount;

    [SerializeField] private GameObject deathScreen;

    public UnityEvent<int /*Score*/> OnDeath;

    private void Start()
    {
        currentPlayerCount = figureFactory.FigureCount;
        figureFactory.OnFigureSpawnedListChanged += UpdatePlayerCount;
    }

    private void UpdatePlayerCount(List<GameObject> obj)
    {
        currentPlayerCount = obj.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        var newFigureCount = figureFactory.RemoveFigure(other.gameObject);
        
        if (newFigureCount <= 0)
        {
            deathScreen.SetActive(true);
            OnDeath?.Invoke(highScore.currentScore);
            
            List<int> scores = Prefs.ScoreSystem.BestScores;
            int newScore = highScore.currentScore;
            
            foreach (int oldScore in scores)
            {
                if (oldScore == newScore) return;
            }
            
            if (newScore > scores[^1])
            {
                int insertIndex = scores.FindIndex(score => newScore > score);
                scores.Insert(insertIndex, newScore);
                
                Prefs.SetKey(Prefs.KEY_TYPES.NEW_SCORE, true);
                
                if (scores.Count > 10)
                    scores.RemoveAt(scores.Count - 1);

                Prefs.ScoreSystem.BestScores = scores;
                // If list count is higher than 10, remove last entry
            }
        }
    }
}