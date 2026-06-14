using UnityEngine;

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
        transform.Rotate(0, 0, steerSpeed);
        transform.Translate(0, moveSpeed, 0);
    }
}
