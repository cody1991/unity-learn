using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    Vector2 moveVector;
    [SerializeField] float torqueAmount = 300f;
    [SerializeField] float baseSpeed = 15f;
    [SerializeField] float boostSpeed = 20f;

    SurfaceEffector2D surfaceEffector2D;

    [SerializeField] bool canControlPlayer = true;

    InputAction moveAction;
    Rigidbody2D rb;
    
    float previousRotation;
    float totalRotation;

    score scoreValue;

    void CalculateFlips() {
        float currentRotation = transform.rotation.eulerAngles.z;

        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation);

        if (Mathf.Abs(totalRotation) > 340) {
            totalRotation = 0;
            scoreValue.AddScore(100);

        }

        previousRotation = currentRotation;

    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();

        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        scoreValue = FindAnyObjectByType<score>();
    }

    public void HandleCanControlPlayer(bool canControlPlayer) {
        this.canControlPlayer = canControlPlayer;
    }

    void Update()
    {
        if (canControlPlayer) {
            rotatePlayer();
            boostPlayer();
        }
        CalculateFlips();
    }

    void rotatePlayer() {
        moveVector = moveAction.ReadValue<Vector2>();
        rb.AddTorque(-moveVector.x * torqueAmount * Time.deltaTime, ForceMode2D.Impulse);
    }

    void boostPlayer() {
        if (moveVector.y > 0) {
            surfaceEffector2D.speed = boostSpeed;
        } else {
            surfaceEffector2D.speed = baseSpeed;
        }
    }

    public void ApplyPowerup(powerup powerup) {

        switch (powerup.GetPowerupName()) {
            case "speed":
                boostSpeed += powerup.GetValueChange();
                baseSpeed += powerup.GetValueChange();
                break;
            case "torque":
                torqueAmount += powerup.GetValueChange();
                break;
        }
    }
}
