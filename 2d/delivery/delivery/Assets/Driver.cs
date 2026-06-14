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

        if (Keyboard.current.wKey.isPressed) {
            Debug.Log("W key is pressed");
        } else if (Keyboard.current.sKey.isPressed) {
            Debug.Log("S key is pressed");
        }
        
        if (Keyboard.current.aKey.isPressed) {
            Debug.Log("A key is pressed");
        } else if (Keyboard.current.dKey.isPressed) {
            Debug.Log("D key is pressed");
        }
        
        transform.Rotate(0, 0, steerSpeed);
        transform.Translate(0, moveSpeed, 0);
    }
}
