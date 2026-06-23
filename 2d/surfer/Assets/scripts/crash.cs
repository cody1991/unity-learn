using UnityEngine;
using UnityEngine.SceneManagement; 

public class crash : MonoBehaviour
{
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] float reloadDelay = 10f;

    player_controller playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<player_controller>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        int layerIndex = LayerMask.NameToLayer("Floor");
        
        if (other.gameObject.layer == layerIndex) {
            Invoke("ReloadScene", reloadDelay);
            crashEffect.Play();
            playerController.changeCanControlPlayer(false);
        }
    }

    void ReloadScene() {
        SceneManager.LoadScene(0);
    }
}
