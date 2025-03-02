using UnityEngine;

public class EnemyThrowBomb : MonoBehaviour
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
            // Vị trí dưới chân Player
            Vector3 targetPosition = new Vector3(player.position.x, 0.1f, player.position.z);

            // Tạo bom
            GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody rb = bomb.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Tính toán lực ném phù hợp với khoảng cách
                Vector3 velocity = CalculateThrowVelocity(throwPoint.position, targetPosition, throwAngle);
                rb.linearVelocity = velocity;
            }

            Destroy(bomb, 5f);
        }
    }

    // Tính toán vận tốc ném dựa trên khoảng cách và góc ném
    private Vector3 CalculateThrowVelocity(Vector3 start, Vector3 target, float angle)
    {
        // Chuyển góc ném từ độ sang radian
        float radianAngle = angle * Mathf.Deg2Rad;

        // Vector hướng từ vị trí ném đến mục tiêu (chỉ tính toán trên mặt phẳng ngang)
        Vector3 direction = target - start;
        direction.y = 0; // Bỏ qua chênh lệch độ cao

        // Khoảng cách ngang từ vị trí ném đến mục tiêu
        float horizontalDistance = direction.magnitude;

        // Độ cao chênh lệch giữa vị trí ném và mục tiêu
        float verticalDistance = target.y - start.y;

        // Tính toán vận tốc ban đầu cần thiết
        float gravity = Physics.gravity.magnitude;
        float velocity = Mathf.Sqrt(
            (gravity * horizontalDistance * horizontalDistance) /
            (2 * (horizontalDistance * Mathf.Tan(radianAngle) - verticalDistance) * Mathf.Pow(Mathf.Cos(radianAngle), 2))
        );

        // Vector vận tốc ban đầu
        Vector3 throwVelocity = direction.normalized * velocity * Mathf.Cos(radianAngle);
        throwVelocity.y = velocity * Mathf.Sin(radianAngle);

        return throwVelocity;
    }

    // Vẽ phạm vi phát hiện trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}