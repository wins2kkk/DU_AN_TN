using UnityEngine;
using UnityEngine.UI;
public class Dragon : MonoBehaviour
{
    private int HP = 100;
    public Slider healthBar;
    public Animator animator;

    public GameObject fireBall;
    public Transform fireBallPoint;

    // Prefab của vật phẩm rơi ra
    public GameObject lootPrefab;
    // Hiệu ứng rơi đồ
    public GameObject lootEffectPrefab;

    // Khoảng cách tối thiểu và tối đa vật phẩm rơi ra
    public float minLootDistance = 2f;
    public float maxLootDistance = 5f;

    //public TaskManager taskManager; // Tham chiếu đến TaskManager
    void Update()
    {
        //healthBar.value = HP;
    }
    void Start()
    {
        // Lấy tham chiếu đến TaskManager (có thể gán từ Inspector nếu cần)
        //taskManager = FindObjectOfType<TaskManager>();
    }
    public void Scream()
    {
       // FindObjectOfType<AudioManager>().Play("DragonScream");
    }

    public void Attack()
    {
       // FindObjectOfType<AudioManager>().Play("DragonAttack");
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            animator.SetTrigger("die");
            GetComponent<Collider>().enabled = false;
            //FindObjectOfType<AudioManager>().Play("DragonDeath");

            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
            }

            //// Gọi TaskManager để đánh dấu nhiệm vụ giết Dragon hoàn thành
            //if (taskManager != null)
            //{
            //    taskManager.CompleteKillDragonTask(); // Gọi để tích nhiệm vụ giết Dragon
            //}

            // Gọi hàm DropLoot sau 1 giây
            Invoke(nameof(DropLoot), 1f);
        }
        else
        {
            //FindObjectOfType<AudioManager>().Play("DragonDamage");
            animator.SetTrigger("damage");
        }
    }

    void DropLoot()
    {
        if (lootPrefab != null)
        {
            Vector3 randomPosition = GetRandomLootPosition();

            if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
            {
                randomPosition = hit.point + Vector3.up * 0.5f;
            }

            GameObject loot = Instantiate(lootPrefab, randomPosition, Quaternion.identity);

            if (lootEffectPrefab != null)
            {
                Instantiate(lootEffectPrefab, randomPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomLootPosition()
    {
        Vector3 randomPosition;
        do
        {
            randomPosition = transform.position + Random.insideUnitSphere * maxLootDistance;
            randomPosition.y = transform.position.y;
        } while (Vector3.Distance(transform.position, randomPosition) < minLootDistance);

        return randomPosition;
    }
}
