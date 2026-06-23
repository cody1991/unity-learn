using UnityEngine;

public class snow : MonoBehaviour
{

    [SerializeField] ParticleSystem snowEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            snowEffect.Play();
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            snowEffect.Stop();
        }
    }
}
