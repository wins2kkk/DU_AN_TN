using UnityEngine;

public class EnemyAiboss : MonoBehaviour
{
    [Header("Cài đặt ném bom")]
    public GameObject bombPrefab;      // Prefab bom
    public Transform throwPoint;       // Vị trí ném bom (tay Enemy)
    public float detectionRange = 10f; // Phạm vi phát hiện
    public float throwAngle = 45f;     // Góc ném (độ)

    [Header("Thời gian giữa các lần ném")]
    public float throwCooldown = 3f;
    private float nextThrowTime;

    private Transform player;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Phát hiện Player trong phạm vi và kích hoạt animation
        if (distanceToPlayer <= detectionRange && Time.time >= nextThrowTime)
        {
            transform.LookAt(player); // Quay về phía Player
            animator.SetTrigger("Throw"); // Kích hoạt animation ném
            nextThrowTime = Time.time + throwCooldown; // Cập nhật thời gian ném tiếp theo
        }
    }

    // Hàm này sẽ được gọi bằng Animation Event
    public void ThrowBomb()
    {
        if (bombPrefab != null && throwPoint != null && player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, 0.1f, player.position.z);

            // Tạo và ném 3 quả bom với góc lệch nhau
            for (int i = -1; i <= 1; i++)
            {
                GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
                Rigidbody rb = bomb.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 adjustedTarget = targetPosition + new Vector3(i * 1.5f, 0, i * 1.5f); // Dịch vị trí mục tiêu sang hai bên
                    Vector3 velocity = CalculateThrowVelocity(throwPoint.position, adjustedTarget, throwAngle);
                    rb.linearVelocity = velocity;
                }

                Destroy(bomb, 5f);
            }
        }
    }

    // Tính toán vận tốc ném dựa trên khoảng cách và góc ném
    private Vector3 CalculateThrowVelocity(Vector3 start, Vector3 target, float angle)
    {
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector3 direction = target - start;
        direction.y = 0;
        float horizontalDistance = direction.magnitude;
        float verticalDistance = target.y - start.y;
        float gravity = Physics.gravity.magnitude;
        float velocity = Mathf.Sqrt(
            (gravity * horizontalDistance * horizontalDistance) /
            (2 * (horizontalDistance * Mathf.Tan(radianAngle) - verticalDistance) * Mathf.Pow(Mathf.Cos(radianAngle), 2))
        );
        Vector3 throwVelocity = direction.normalized * velocity * Mathf.Cos(radianAngle);
        throwVelocity.y = velocity * Mathf.Sin(radianAngle);
        return throwVelocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
