using UnityEngine;

public class CoinPickUp : MonoBehaviour
{

    [SerializeField] AudioClip coinPickUpSFX;
    [SerializeField] int pointsForCoinPickUp = 100;


    bool wasCollected = false;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !wasCollected) {
            // 死亡动画期间玩家被弹飞，仍可能撞到金币，这里排除掉
            PlayerMove player = other.GetComponent<PlayerMove>();
            if (player != null && !player.IsAlive) {
                return;
            }

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
 