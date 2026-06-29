using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        PlayerMove player = other.GetComponent<PlayerMove>();
        if (player == null) {
            return;
        }

        // 获取当前场景索引
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 加载下一个场景，但是如果当前场景是最后一个场景，就不做任何操作
        if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1) {
            player.ClearSave();
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
