using UnityEngine;

public class savePosition : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        PlayerMove player = other.GetComponent<PlayerMove>();
        if (player != null) {
            player.SavePosition(transform.position);
        }
    }
}
