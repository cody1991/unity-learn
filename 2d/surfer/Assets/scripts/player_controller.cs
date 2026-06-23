using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    Vector2 moveVector;
    [SerializeField] float torqueAmount = 1f;
    [SerializeField] float baseSpeed = 15f;
    [SerializeField] float boostSpeed = 20f;

    SurfaceEffector2D surfaceEffector2D;

    InputAction moveAction;
    Rigidbody2D rb;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();

        surfaceEffector2D = FindObjectOfType<SurfaceEffector2D>();
    }

    void Update()
    {
       rotatePlayer();
       boostPlayer();
    }

    void rotatePlayer() {
        moveVector = moveAction.ReadValue<Vector2>();
        rb.AddTorque(-moveVector.x * torqueAmount * Time.deltaTime); 
    }

    void boostPlayer() {
        if (moveVector.y > 0) {
            surfaceEffector2D.speed = boostSpeed;
        } else {
            surfaceEffector2D.speed = baseSpeed;
        }
    }
}
