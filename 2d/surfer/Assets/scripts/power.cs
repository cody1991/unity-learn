using UnityEngine;

public class power : MonoBehaviour
{
    [SerializeField] powerup powerup;
    
    player_controller playerController;

    void Start() {
        playerController = FindAnyObjectByType<player_controller>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playerController.ApplyPowerup(powerup);
            
        }
    }
}
