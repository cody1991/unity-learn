using UnityEngine;
using TMPro;

public class score : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI scoreText;

    int scoreValue = 0;

    public void AddScore(int amount) {
        scoreValue += amount;
        scoreText.text = "Score: " + scoreValue;
    }

}
