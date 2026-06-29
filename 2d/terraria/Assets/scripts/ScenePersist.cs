using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake() {

        int numberScenePersists = FindObjectsByType<ScenePersist>(FindObjectsSortMode.None).Length;

        Debug.Log("Number of scene persists: " + numberScenePersists);

        if (numberScenePersists > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }

    }

    public void ResetScenePersist() {
        Destroy(gameObject);
    }
}
