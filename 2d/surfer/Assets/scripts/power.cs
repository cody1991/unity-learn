using UnityEngine;

public class power : MonoBehaviour
{
    [SerializeField] powerup powerup;
    
    player_controller playerController;

    SpriteRenderer spriteRenderer;

    void Start() {
        playerController = FindAnyObjectByType<player_controller>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && spriteRenderer.enabled) {
            playerController.ApplyPowerup(powerup);
            spriteRenderer.enabled = false;
        }
    }
}
