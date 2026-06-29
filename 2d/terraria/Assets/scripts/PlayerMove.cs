using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 20f);

    // 存档放在静态变量（内存）里：场景重载（死亡复活）时还在，刷新浏览器时会被清空
    static bool hasSavedPosition = false;
    static Vector2 savedPosition;

    Rigidbody2D myRigidbody;
    Vector2 moveInput;

    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    bool isAlive = true;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun; 

    void Start()
    {
        // 获取 Rigidbody2D 组件
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        

        gravityScaleAtStart = myRigidbody.gravityScale;
        LoadPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();

        Die();

    }

    

    void OnMove(InputValue value) {
        if (!isAlive) {
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        if (!isAlive) {
            return;
        }
        // 跳跃逻辑
        bool isGround = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
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
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
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
    
    void OnAttack(InputValue value) {
        if (!isAlive) {
            return;
        }
        if (!value.isPressed) {
            return;
        }
        // instantiate 实例化一个游戏对象, 第一个参数是游戏对象, 第二个参数是位置, 第三个参数是旋转
        // gun.position 是枪的位置, 它大概是枪的中心位置
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void Die() {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("enemy", "hazards", "Water"))) {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.linearVelocity = deathKick;

            // 三秒后重启
            Invoke("RestartGame", 1f);
        }
    }


    void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SavePosition() {
        // 按钮调用这里保存安全位置，死亡重载后会从这个位置恢复
        savedPosition = transform.position;
        hasSavedPosition = true;
    }

    public void LoadPosition() {
        // 没有存档时使用场景里摆放的出生位置
        if (!hasSavedPosition) {
            return;
        }

        transform.position = new Vector3(savedPosition.x, savedPosition.y, transform.position.z);
    }

    public void ClearSave() {
        hasSavedPosition = false;
    }
}
