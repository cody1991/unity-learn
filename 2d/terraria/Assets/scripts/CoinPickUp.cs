using UnityEngine;

public class CoinPickUp : MonoBehaviour
{

    [SerializeField] AudioClip coinPickUpSFX;
    [SerializeField] int pointsForCoinPickUp = 100;


    bool wasCollected = false;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !wasCollected) {
            wasCollected = true;
                        // playclipatpoint的意思是在当前场景中播放音频，而不是在全局播放
            AudioSource.PlayClipAtPoint(coinPickUpSFX, transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject);

            GameSession gameSession = FindAnyObjectByType<GameSession>();
            gameSession.AddToScore(pointsForCoinPickUp);
        }
    }
}
 