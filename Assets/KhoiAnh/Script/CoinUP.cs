using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Coin System/Coin Pickup")]
    public class CoinUP : MonoBehaviour
    {
        public AudioClip pickupCOIN;  // Âm thanh khi nhặt vật phẩm
        [Tooltip("Số coin thu được khi nhặt")]
        public int coinValue = 1; // Số coin tăng khi nhặt (thay vì XP)

        private void OnTriggerEnter(Collider other)
        {
            // Tìm HealthSystem trên người chơi
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Gọi phương thức AddCoins trong HealthSystem để tăng coin
                healthSystem.AddCoins(coinValue);
                if (pickupCOIN!= null)
                {
                    AudioSource.PlayClipAtPoint(pickupCOIN, transform.position);
                }
                Debug.Log($"Player picked up a coin worth {coinValue} coins!");

                // Xóa coin sau khi nhặt (có thể thêm hiệu ứng hoặc âm thanh nếu muốn)
                Destroy(gameObject);
            }
        }
    }
}