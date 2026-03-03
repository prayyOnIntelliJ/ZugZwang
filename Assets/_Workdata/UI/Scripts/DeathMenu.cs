using MyBox;
using TMPro;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    [Separator("Highscore Settings")] [SerializeField] [Tooltip("The Textfield where the highscore will be shown.")]
    private TextMeshProUGUI scoreTextfield;

    [SerializeField] private HighScore highScoreObject;

    private void OnEnable()
    {
        if (scoreTextfield != null && highScoreObject != null)
        {
            scoreTextfield.text = highScoreObject.currentScore.ToString();
        }
    }
}