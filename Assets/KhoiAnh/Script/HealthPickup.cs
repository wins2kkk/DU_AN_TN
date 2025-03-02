using Akila.FPSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healingAmount = 25f; // Lượng máu hồi phục
    public AudioClip pickupSound;  // Âm thanh khi nhặt vật phẩm

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem playerHealth = other.GetComponent<HealthSystem>();

        if (playerHealth != null)
        {
            Debug.Log("Health Pickup Triggered!");  // ✅ Kiểm tra xem có va chạm không
            Debug.Log("Healing Amount: " + healingAmount);

            playerHealth.Heal(healingAmount);

            Debug.Log("New Health: " + playerHealth.GetHealth()); // ✅ Kiểm tra giá trị máu sau khi hồi

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }

}
