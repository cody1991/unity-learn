using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{

    [SerializeField] float loadDelay = 1f;
    void OnTriggerEnter2D(Collider2D other) {
        PlayerMove player = other.GetComponent<PlayerMove>();
        StartCoroutine(LoadNextScene(player));
    }

    IEnumerator LoadNextScene(PlayerMove player) {
        // 开启协程后，会等待1秒后执行下面的代码
        yield return new WaitForSecondsRealtime(loadDelay);
         // 获取当前场景索引
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // 加载下一个场景，但是如果当前场景是最后一个场景，就不做任何操作
        if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1) {
            player.ClearSave();

            ScenePersist scenePersist = FindFirstObjectByType<ScenePersist>();
            if (scenePersist != null) {
                scenePersist.ResetScenePersist();
            }

            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
