using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FruitBoundaryClamp : MonoBehaviour
{
    [SerializeField] float wallInnerX = 2.38f;

    public void Configure(float innerX)
    {
        wallInnerX = innerX;
    }

    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    Fruit fruit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        fruit = GetComponent<Fruit>();
    }

    void FixedUpdate()
    {
        if (fruit == null || fruit.IsPreview || circleCollider == null)
        {
            return;
        }

        float radius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Vector2 position = rb.position;
        float minX = -wallInnerX + radius;
        float maxX = wallInnerX - radius;
        float clampedX = Mathf.Clamp(position.x, minX, maxX);

        if (Mathf.Approximately(clampedX, position.x))
        {
            return;
        }

        position.x = clampedX;
        rb.position = position;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}
