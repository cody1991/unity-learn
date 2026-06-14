using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] float steerSpeed = 2f; // 转向速度
    [SerializeField] float moveSpeed = 0.1f; // 移动速度

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        
        transform.Rotate(0, 0, steer * steerSpeed);
        transform.Translate(0, move * moveSpeed, 0);
    }
}
