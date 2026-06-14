using UnityEngine;

public class Collision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision with " + other.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger with " + other.gameObject.name);
    }
}
