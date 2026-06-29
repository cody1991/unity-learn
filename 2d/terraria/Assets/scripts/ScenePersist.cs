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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
