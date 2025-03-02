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
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        // Thêm trường cho coin
        [Header("Coin Settings")]
        [Tooltip("The UI Text element to display the player's coins")]
        public TextMeshProUGUI coinText; // Gán Text UI trong Inspector để hiển thị số coin
        private int currentCoins = 0; // Số coin hiện tại của người chơi

        // Thêm trường mới cho kinh nghiệm và level
        [Header("Experience and Level Settings")]
        [Tooltip("The UI Text element to display the player's experience")]
        public Text experienceText; // Gán Text UI trong Inspector
        [Tooltip("The UI Text element to display the player's level")]
        public Text levelText; // Gán Text UI trong Inspector
        private float currentExperience = 0f; // Tổng kinh nghiệm của player
        private int currentLevel = 1; // Level hiện tại, bắt đầu từ 1
        private float experienceToNextLevel = 1000f; //Kinh nghiệm để lên được Level
        private AudioSource audioSource;
        public AudioClip levelUpSound; // Gán AudioClip trong Inspector

        // Thêm trường này
        public Text playTimeText;
        private float playTime = 0f;

        //khai bao Slider 
        public Slider healthBar;

        public HealthType type = HealthType.Other;
        public float health = 100;
        public float destroyDelay;
        [Range(0, 1)] public float damageCameraShake = 0.3f;

        [Space]
        public bool destoryOnDeath;
        public bool destroyRoot;
        public bool ragdolls;
        public GameObject deathEffect;
        //public DeathPanelManager deathPanelManager;
        [Space]
        public UnityEvent OnDeath;

        public Actor Actor { get; set; }
        public Ragdoll ragdoll { get; set; }
        private Actor killer;
        private DeathCamera deathCamera;
        public Vector3 deathForce { get; set; }
        public float MaxHealth { get; set; }
        public IDamageableGroup[] groups { get; set; }
        private bool died;
        public bool deadConfirmed { get; set; }
        private bool isDead = false; // Biến kiểm soát input


        public GameObject deathPanel; // Panel thông báo chết, kéo từ Inspector
        private CanvasGroup canvasGroup;

        public string gameOverScene = "LosseGame"; // Scene sẽ chuyển đến sau khi chết

        private void Start()
        {
            if (deathPanel != null)
            {
                canvasGroup = deathPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = deathPanel.AddComponent<CanvasGroup>();

                canvasGroup.alpha = 0; // Ẩn panel lúc đầu
                deathPanel.SetActive(false);
            }

            // Khởi tạo AudioSource level up
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (healthBar != null)
            {
                healthBar.value = health; // ✅ Cập nhật giá trị máu
            }

            Actor = GetComponent<Actor>();
            ragdoll = GetComponent<Ragdoll>();

            OnDeath.AddListener(Die);
            if (FindObjectOfType<GameManager>()) deathCamera = FindObjectOfType<GameManager>().DeathCamera;

            MaxHealth = health;

            if (type == HealthType.Humanoid)
            {
                if (Actor && Actor.characterManager != null) deathCamera?.Disable();

                groups = GetComponentsInChildren<IDamageableGroup>();

                if (Actor && Actor.characterManager != null)
                {
                    if (UIManager.Instance && UIManager.Instance.HealthDisplay && Actor)
                    {
                        UIManager.Instance.HealthDisplay?.UpdateCard(GetHealth(), Actor.actorName, false);
                        UIManager.Instance.HealthDisplay.actorNameText.text = Actor.actorName;
                    }
                }
            }

            if (type == HealthType.Other)
            {
                if (ragdoll || Actor) Debug.LogWarning($"{this} has humanoid components and it's type is Other please change type to Humanoid to avoid errors.");
            }

            // Khởi tạo UI kinh nghiệm và level
            UpdateExperienceUI();
            UpdateLevelUI();
            UpdateCoinUI();
        }

        private void Update()
        {
            // Cập nhật thời gian chơi
            playTime += Time.deltaTime;

            if (playTimeText != null)
            {
                int minutes = Mathf.FloorToInt(playTime / 60f);
                int seconds = Mathf.FloorToInt(playTime % 60f);
                playTimeText.text = string.Format("TIME: {0:00}:{1:00}", minutes, seconds);
            }

            //// Các logic khác của HealthSystem
            //UpdateSystem();
        }
        private void UpdateSystem()
        {
            if (healthBar != null)
            {
                healthBar.value = health; // ✅ Cập nhật UI
            }

            if (!died && health <= 0)
            {
                died = true;
                OnDeath?.Invoke(); // Gọi sự kiện chết
                StartCoroutine(DieSequence()); // Chờ 3 giây rồi load Scene
            }

            UpdateUI(1);
        }


        public void ResetPlayerData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("Player data reset!");

            // Đặt lại giá trị mặc định
            currentLevel = 1;
            currentCoins = 0;
            playTime = 0f;
            currentExperience = 0f;
            MaxHealth = 100f;
            health = MaxHealth;

            UpdateLevelUI();
            UpdateCoinUI();
            UpdateExperienceUI();
        }

        public void Heal(float heal)
        {
            health += heal;

            if (health > MaxHealth)
            {
                health = MaxHealth; // Đảm bảo không vượt quá giới hạn máu
            }

            Debug.Log("Healed: " + heal + " | Current Health: " + health);

            UpdateSystem(); // ✅ Gọi UpdateSystem() để cập nhật UI và kiểm tra trạng thái
        }


        public void DoDamage(float damage, Actor killer)
        {
            health -= damage;
            this.killer = killer;

            if (type == HealthType.Humanoid && Actor.characterManager != null)
            {
                Actor.characterManager.cameraManager.ShakeCameras(damageCameraShake);
            }

            UpdateSystem();
        }

        private void UpdateUI(float alpha)
        {

            if (type == HealthType.Humanoid && Actor.characterManager != null)
            {
                UIManager.Instance.DamageIndicator.Show(alpha);
                UIManager.Instance.HealthDisplay.UpdateCard(health, Actor.actorName, true);
            }
        }




        private IEnumerator DieSequence()
        {
            died = true;

            CharacterInput characterInput = GetComponent<CharacterInput>();
            if (characterInput) characterInput.isDead = true;

            if (ragdoll) ragdoll.Enable(deathForce);
            if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);

            if (deathCamera)
            {
                if (killer)
                    deathCamera.Enable(Actor, killer);
                else
                {
                    deathCamera.transform.position = transform.position + Vector3.up * 2;
                    deathCamera.transform.rotation = Quaternion.identity;
                    deathCamera.Camera.enabled = true;
                    deathCamera.AudioListener.enabled = true;
                }
            }

            died = true;

            // Lưu level, coin và thời gian chơi trước khi chuyển scene
            PlayerPrefs.SetInt("PlayerLevel", currentLevel);
            PlayerPrefs.SetInt("PlayerCoins", currentCoins);
            PlayerPrefs.SetFloat("PlayerTime", playTime);
            PlayerPrefs.Save(); // Lưu dữ liệu vào bộ nhớ

            yield return new WaitForSeconds(2.5f);

            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
                yield return StartCoroutine(FadeInPanel());
            }

            yield return new WaitForSeconds(4.5f);

            SceneManager.LoadScene(gameOverScene);
          //  SceneManager.LoadScene("Winnnn");
        }

        // 🎨 Hiệu ứng mờ từ 0 -> 1 trong 5s
        private IEnumerator FadeInPanel()
        {
            float duration = 1.0f; // Thời gian fade-in
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        private void Die()
        {
            StartCoroutine(DieSequence());
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Key"))
            {
                SavePlayerData(); // Lưu dữ liệu điểm số
                StartCoroutine(LoadWinSceneAfterDelay(5f)); // Chuyển scene sau 5 giây
            }
        }

        private IEnumerator LoadWinSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene("WinGame"); // Thay bằng tên scene thắng của bạn
        }


        private void SavePlayerData()
        {
            PlayerPrefs.SetInt("PlayerLevel", currentLevel);
            PlayerPrefs.SetInt("PlayerCoins", currentCoins);
            PlayerPrefs.SetFloat("PlayerTime", playTime);
            PlayerPrefs.Save(); // Lưu dữ liệu vào bộ nhớ
        }


        private void Respwan()
        {
            // Tắt hồi sinh bằng cách không thực hiện bất kỳ hành động nào trong phương thức này
            // if (type != HealthType.Humanoid && !Actor) return;

            // if (Actor.actorManager && Actor.actorManager.respawnable)
            //     Actor.actorManager.Respwan(Actor.actorManager.SpwanManager.respawnDelay);

            // Tắt camera khi chết
            // if (Actor.characterManager != null) deathCamera.Enable(Actor, killer);
        }

        public float GetHealth()
        {
            return health;
        }

        public void Damage(float amount, Actor damageSource)
        {
            DoDamage(amount, damageSource);
        }

        public bool IsDead()
        {
            return health <= 0 ? true : false;
        }

        public Actor GetActor()
        {
            return Actor;
        }

        public int GetGroupsCount()
        {
            if (groups != null) return groups.Length;

            return 0;
        }

        public Ragdoll GetRagdoll()
        {
            return ragdoll;
        }

        // Thêm hàm xử lý kinh nghiệm
        public void AddExperience(float amount)
        {
            currentExperience += amount;
            Debug.Log($"Player gained {amount} XP. Total XP: {currentExperience}");

            // Kiểm tra xem có đủ kinh nghiệm để lên level không
            while (currentExperience >= experienceToNextLevel)
            {
                LevelUp();
            }

            UpdateExperienceUI();
        }

        // Hàm xử lý lên level
        private void LevelUp()
        {
            currentLevel++; // Tăng level
            MaxHealth = currentLevel * 100; // Máu tối đa = level * 100
            health = MaxHealth; // Hồi đầy máu khi lên level
            experienceToNextLevel += 1000; // Tăng mốc kinh nghiệm cần cho level tiếp theo

            Debug.Log($"Leveled up to level {currentLevel}. Max Health: {MaxHealth}");

            // Cập nhật UI
            UpdateLevelUI();
            UpdateSystem(); // Cập nhật health bar

            // Phát âm thanh lên cấp
            if (audioSource != null && levelUpSound != null)
            {
                audioSource.PlayOneShot(levelUpSound);
            }
        }

        // Cập nhật UI kinh nghiệm
        private void UpdateExperienceUI()
        {
            if (experienceText != null)
            {
                experienceText.text = $"XP: {currentExperience}/{experienceToNextLevel}";
            }
            else
            {
                Debug.LogWarning("Experience Text is not assigned in the Inspector!");
            }
        }

        // Cập nhật UI level
        private void UpdateLevelUI()
        {
            if (levelText != null)
            {
                levelText.text = $"LEVEL: {currentLevel}";
            }
            else
            {
                Debug.LogWarning("Level Text is not assigned in the Inspector!");
            }
        }

        // Hàm xử lý coin
        public void AddCoins(int amount)
        {
            currentCoins += amount;
            Debug.Log($"Player collected {amount} coins. Total coins: {currentCoins}");

            // Cập nhật UI coin
            UpdateCoinUI();
        }

        // Cập nhật UI coin
        private void UpdateCoinUI()
        {
            if (coinText != null)
            {
                coinText.text = $"RUBY: {currentCoins}";
            }
            else
            {
                Debug.LogWarning("Coin Text is not assigned in the Inspector!");
            }
        }
        // Getter cho kinh nghiệm
        public float GetCurrentExperience()
        {
            return currentExperience;
        }

        // Getter cho level
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
    }



    public enum HealthType
    {
        Humanoid = 0,
        Other = 1
    }
}
