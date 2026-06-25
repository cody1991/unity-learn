using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    Rigidbody2D myRigidbody;
    Vector2 moveInput;

    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;

    float gravityScaleAtStart;

    void Start()
    {
        // 获取 Rigidbody2D 组件
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();

        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        // 跳跃逻辑
        bool isGround = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!isGround) {
            return;
        }
        if (value.isPressed) {
             myRigidbody.linearVelocity += new Vector2(0f, jumpSpeed);
        }
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

    void ClimbLadder() {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false); 
            return;
        }
        Vector2 climbVelocity = new Vector2(myRigidbody.linearVelocity.x, moveInput.y * climbSpeed);
        myRigidbody.linearVelocity = climbVelocity;

        myRigidbody.gravityScale = 0f;

        bool isClimbing = Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", isClimbing);
    }
}
