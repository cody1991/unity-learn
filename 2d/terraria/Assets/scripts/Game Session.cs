using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;

    // 单例模式
    // 比如减少生命值
    void Awake() {

        int numberGameSessions = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;

        Debug.Log("Number of game sessions: " + numberGameSessions);

        if (numberGameSessions > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }

    }

    public void ProcessPlayerDeath() {
        if (playerLives > 1) {
            TakeLife();
        } else {
            ResetGameSession();
        }
    }

    void TakeLife() {
        playerLives--;
        ReloadCurrentScene();
    }

    void ResetGameSession() {
        playerLives = 3;
        ReloadCurrentScene();

        ScenePersist scenePersist = FindFirstObjectByType<ScenePersist>();
        if (scenePersist != null) {
            scenePersist.ResetScenePersist();
        }
    }

    public void AddToScore(int pointsToAdd) {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    void ReloadCurrentScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        livesText.text = playerLives.ToString();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
