using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;

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
    }

    void ReloadCurrentScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
