using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] float steerSpeed = 100f; // 转向速度
    [SerializeField] float currentSpeed = 5f; // 移动速度

    [SerializeField] float boostSpeed = 10f;
    [SerializeField] float regularSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Boost")) {
            currentSpeed = boostSpeed;
            Destroy(other.gameObject);
        } 
    }

    void OnCollisionEnter2D(Collision2D other) {
        currentSpeed = regularSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float steer = 0;
        float move = 0;

        if (Keyboard.current.wKey.isPressed) {
            move = 1;
        } else if (Keyboard.current.sKey.isPressed) {
            move = -1;
        }

        if (Keyboard.current.aKey.isPressed) {
            steer = 1;
        } else if (Keyboard.current.dKey.isPressed) {
            steer = -1;
        }

        // 碰撞到 boost
        
        
        float steerAmount = steer * steerSpeed * Time.deltaTime;
        float moveAmount = move * currentSpeed * Time.deltaTime;

        transform.Rotate(0, 0, steerAmount);
        transform.Translate(0, moveAmount, 0);
    }
}
