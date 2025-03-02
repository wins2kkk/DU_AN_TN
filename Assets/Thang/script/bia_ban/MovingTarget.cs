using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    public float speed = 3f; // Tốc độ di chuyển
    public float range = 5f; // Khoảng cách di chuyển tối đa

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển qua lại trên trục Z
        transform.position += new Vector3(0, 0, speed * direction * Time.deltaTime);

        // Nếu vượt quá phạm vi, đổi hướng
        if (Mathf.Abs(transform.position.z - startPosition.z) >= range)
        {
            direction *= -1;
        }
    }
}
