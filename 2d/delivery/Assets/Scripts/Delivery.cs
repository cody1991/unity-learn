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

    bool hasPackage = false;

    void OnCollisionEnter2D(Collision2D other) {
        // Debug.Log("Collision with " + other.gameObject.name + " " + other.gameObject.tag);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("Trigger with " + other.gameObject.name + " " + other.gameObject.tag);
        if (other.CompareTag("Package")) {
            // Debug.Log("Package picked up"); 
            Destroy(other.gameObject);
            hasPackage = true;
        }
        if (other.CompareTag("Customer") && hasPackage) {
            Debug.Log("Package delivered");
            hasPackage = false;
        }
    }
}
 