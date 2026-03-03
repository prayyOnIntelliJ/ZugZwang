using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class HighscoreMenu : MonoBehaviour
{
    [Separator("HighScore Settings")]
    [SerializeField] private List<TextMeshProUGUI> rankTextFields;
    [SerializeField] private List<TextMeshProUGUI> scoreTextFields;
    [Separator("Texts")] 
    [SerializeField] private string noHighScoresText = "No highscores yet.";

    private void Start()
    {
        List<int> highScores = Prefs.ScoreSystem.BestScores;

        for (int i = 0; i < highScores.Count; i++)
        {
            int rankIndex = i + 1;
            if (highScores[i] == 0 || highScores == null)
                scoreTextFields[i].text = noHighScoresText;
            else
            {
                scoreTextFields[i].text = highScores[i].ToString();
            }
            rankTextFields[i].text = rankIndex + ".";
        }
    }
}