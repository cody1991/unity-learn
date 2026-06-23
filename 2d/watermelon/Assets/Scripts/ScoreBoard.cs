using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text bestText;

    int score;
    int bestScore;

    const string BestScoreKey = "WatermelonBestScore";

    void Awake()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
        }

        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (bestText != null)
        {
            bestText.text = "Best: " + bestScore;
        }
    }
}
