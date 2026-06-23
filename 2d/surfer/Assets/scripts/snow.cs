using UnityEngine;

public class snow : MonoBehaviour
{

    [SerializeField] ParticleSystem snowEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        snowEffect.Play();
    }

    void OnCollisionExit2D(Collision2D collision) {
        snowEffect.Stop();
    }
}
