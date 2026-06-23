using UnityEngine;

public class power : MonoBehaviour
{
    [SerializeField] powerup powerup;
    
    player_controller playerController;

    SpriteRenderer spriteRenderer;

    float timeLeft;

    void Start() {
        playerController = FindAnyObjectByType<player_controller>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        timeLeft = powerup.GetTime();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && spriteRenderer.enabled) {
            playerController.ApplyPowerup(powerup);
            spriteRenderer.enabled = false;
        }
    }

    void Update() {
        Countdown();
    }

    void Countdown() {
        if (!spriteRenderer.enabled) {

            if (timeLeft > 0) {
                timeLeft -= Time.deltaTime;

                if (timeLeft <= 0) {
                    // time left
                    Debug.Log("Time left: " + timeLeft);
                }
            }
        }
    }
}
 