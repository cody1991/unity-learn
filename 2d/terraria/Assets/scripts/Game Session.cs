using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;

    // 单例模式
    // 比如减少生命值
    void Awake() {

        int numberGameSessions = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;

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
        ResetGameSession();
    }

    void ResetGameSession() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Destroy(gameObject);
        playerLives = 3;
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
