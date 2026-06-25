using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float runSpeed = 10f;
    Rigidbody2D myRigidbody;
    Vector2 moveInput;

    void Start()
    {
        // 获取 Rigidbody2D 组件
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void Run() {
        // y 不控制，按照 rigidbody 的 y 速度
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
    }
}
