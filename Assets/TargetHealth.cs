using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Health System")]
    public class TargetHealth : MonoBehaviour
    {
        public Slider healthBar;    
        public float health = 100;
        private bool isDead = false;
        private Quaternion originalRotation; // Lưu trạng thái xoay ban đầu

        private void Start()
        {
            if (healthBar != null)
            {
                healthBar.value = health;
            }

            // Lưu trạng thái xoay ban đầu của tấm bia
            originalRotation = transform.rotation;
        }

        public void DoDamage(float damage, Actor killer)
        {
            if (isDead) return; // Nếu đã chết, không nhận sát thương nữa

            health -= damage;

            if (healthBar != null)
            {
                healthBar.value = health;
            }

            if (health <= 0)
            {
                StartCoroutine(DieSequence());
            }
        }

        private IEnumerator DieSequence()
        {
            isDead = true;

            // Xoay tấm bia nghiêng ra sau 100 độ
            Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 100, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = targetRotation;

            yield return new WaitForSeconds(3f); // Chờ 3 giây

            // Từ từ xoay lại vị trí ban đầu trong 1 giây
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                transform.rotation = Quaternion.Lerp(targetRotation, originalRotation, elapsedTime / 1f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Đảm bảo về đúng góc ban đầu
            transform.rotation = originalRotation;

            isDead = false; // Cho phép nhận damage lại nếu cần
            health = 100; // Reset máu nếu muốn tấm bia có thể bị bắn lại
        }
    }
}
