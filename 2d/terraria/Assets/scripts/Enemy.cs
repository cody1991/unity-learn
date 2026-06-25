using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;


    BoxCollider2D myBodyCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // linearVelocity 是 Rigidbody2D 的属性，用于设置物体的线性增量速度
        myRigidbody.linearVelocity = new Vector2(moveSpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D other) {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector2((Mathf.Sign(moveSpeed)), 1f);
    }

}
