using UnityEngine;
using UnityEngine.SceneManagement; 
public class finish_line : MonoBehaviour

{
    [SerializeField] ParticleSystem finishEffect;
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

        int layerIndex = LayerMask.NameToLayer("Player");
        
        if (other.gameObject.layer == layerIndex) {
            Invoke("ReloadScene", reloadDelay);
            finishEffect.Play();
        }
    }

    void ReloadScene() {
        SceneManager.LoadScene(0);
    }
}
