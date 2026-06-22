using UnityEngine;

public class finish_line : MonoBehaviour
{
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
            Debug.Log("Player finished");
        }
    }
}
