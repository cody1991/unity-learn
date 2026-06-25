using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float runSpeed = 10f;
    Rigidbody2D myRigidbody;
    Vector2 moveInput;

    Animator myAnimator;

    void Start()
    {
        // 获取 Rigidbody2D 组件
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void Run() {
        // y 不控制，按照 rigidbody 的 y 速度
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;

        bool isRunning = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;

        myAnimator.SetBool("isRunning", isRunning);
    }

    void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon; // 是否存在水平速度
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x), 1f);
        }
    }
}
