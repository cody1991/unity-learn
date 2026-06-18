using UnityEngine;

public class Delivery : MonoBehaviour
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
        // Debug.Log("Collision with " + other.gameObject.name + " " + other.gameObject.tag);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("Trigger with " + other.gameObject.name + " " + other.gameObject.tag);
        if (other.CompareTag("Package")) {
            // Debug.Log("Package picked up"); 
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Customer")) {
            Debug.Log("Package delivered");
            Destroy(other.gameObject);
        }
    }
}
 