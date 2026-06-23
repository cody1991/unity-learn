using UnityEngine;
using UnityEngine.SceneManagement; 

public class crash : MonoBehaviour
{
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] float reloadDelay = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        int layerIndex = LayerMask.NameToLayer("Floor");
        
        if (other.gameObject.layer == layerIndex) {
            Invoke("ReloadScene", reloadDelay);
            crashEffect.Play();
        }
    }

    void ReloadScene() {
        SceneManager.LoadScene(0);
    }
}
