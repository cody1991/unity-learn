using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] TextMeshProUGUI hudText;

    // 进入当前关卡时的分数存档点：命全部用光、重开本关时回退到这个值，
    // 这样只清掉本关拿到的分，之前关卡累计的分保留
    int scoreAtLevelStart = 0;

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
        ScenePersist scenePersist = FindFirstObjectByType<ScenePersist>();
        if (scenePersist != null) {
            scenePersist.ResetScenePersist();
        }

        // 彻底重开本关：清掉中途的位置存档，让玩家回到关卡默认出生点
        PlayerMove player = FindFirstObjectByType<PlayerMove>();
        if (player != null) {
            player.ClearSave();
        }

        playerLives = 3;
        score = scoreAtLevelStart;
        ReloadCurrentScene();
    }

    public void AddToScore(int pointsToAdd) {
        score += pointsToAdd;
        RefreshUI();
    }

    // 进入新关卡时调用，把当前分数记为本关的存档点
    public void SetLevelStartScore() {
        scoreAtLevelStart = score;
    }

    void ReloadCurrentScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        RefreshUI();
    }

    void RefreshUI() {
        // 一个文本里同时显示生命和分数；想换格式/换行/标签在这里改即可
        hudText.text = "LIVES " + playerLives + " / SCORE " + score;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetLevelStartScore();
        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
