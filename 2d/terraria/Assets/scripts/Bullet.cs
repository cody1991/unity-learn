using UnityEngine;

public class Bullet : MonoBehaviour
{
   Rigidbody2D myRigidbody;
   [SerializeField] float bulletSpeed = 20f;

   PlayerMove player;
   float xSpeed;

   void Start() {
    myRigidbody = GetComponent<Rigidbody2D>();
    player = FindFirstObjectByType<PlayerMove>();
    xSpeed = player.transform.localScale.x * bulletSpeed;
   }

   void OnCollisionEnter2D(Collision2D other) {
    Destroy(gameObject, 1f);
   }

   void Update() {

     myRigidbody.linearVelocity = new Vector2(xSpeed, 0f);
   }

   void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("enemy")) {
        Destroy(other.gameObject);
    }
    Destroy(gameObject); 
   }
}
