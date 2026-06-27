using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Destroy(gameObject);
        }
    }
}
