using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Pickups/Experience Pickup")]
    public class KinhNghiem : MonoBehaviour
    {
        public AudioClip pickupKINHNGHIEM;  // Âm thanh khi nhặt vật phẩm

        [Header("Experience Settings")]
        [Tooltip("The amount of experience points to give the player")]
        [SerializeField] private float experienceAmount = 10f; // Có thể chỉnh trong Inspector

        [Header("Pickup Settings")]
        [Tooltip("Whether to destroy the pickup object after being collected")]
        [SerializeField] private bool destroyOnPickup = true;

        private void OnTriggerEnter(Collider other)
        {
            // Kiểm tra xem có phải player (HealthSystem) không
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                // Gọi hàm trong HealthSystem để thêm kinh nghiệm
                playerHealth.AddExperience(experienceAmount);
                if (pickupKINHNGHIEM != null)
                {
                    AudioSource.PlayClipAtPoint(pickupKINHNGHIEM, transform.position);
                }
                // Hủy object nếu được bật
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}