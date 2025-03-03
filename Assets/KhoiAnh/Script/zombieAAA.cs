using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Zombie Health System")]
    public class zombieAAA : MonoBehaviour, IDamageable
    {
        [Header("Drop Ammo")]
        public GameObject ammoPrefab; // Prefab sẽ được sinh ra khi zombie chết

        [Header("Drop KinhNghiem")]
        public GameObject kinhNghiem; // Prefab sẽ được sinh ra khi zombie chết

        // Thêm mục Sound Settings Normal
        [Header("Ambient Sound Settings")]
        [SerializeField] private float damageCooldown = 3f; // Thời gian chờ sau khi nhận damage để phát lại âm thanh nền
        private float timeWithoutDamage; // Thời gian không nhận damage
        private bool canPlayAmbientSound = true;
        [SerializeField] private AudioClip ambientSound; // Âm thanh nền
        [SerializeField] private float ambientSoundDelay = 5f; // Thời gian chờ giữa các lần phát
        [SerializeField] private float ambientSoundVolume = 0.5f; // Độ lớn âm thanh nền
        private AudioSource audioSourceAmbient;
        private Coroutine ambientSoundCoroutine;

        // Thêm mục Audio Settings Damage
        [Header("Audio Settings")]
        [SerializeField] private AudioClip damageSound; // Âm thanh khi nhận damage
        [SerializeField] private float soundVolume = 1f; // Độ lớn âm thanh
        private AudioSource audioSource;

        //khai bao Slider 
        public Slider healthBar;
        private Transform mainCamera;

        [Header("Explosion Settings")]
        public Explosive explosivePrefab; // Prefab chứa script Explosive
        public float explosionDelay = 0.1f; // Độ trễ trước khi nổ
        public bool inheritVelocity; // Kế thừa vận tốc từ zombie

        public HealthType type = HealthType.Humanoid;
        public float health = 100;
        public float destroyDelay;
        [Range(0, 1)] public float damageCameraShake = 0.3f;

        [Space]
        public bool destoryOnDeath;
        public bool destroyRoot;
        public bool ragdolls;
        public GameObject deathEffect;

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
        private Animator animator;

        private void Start()
        {
            // Khởi tạo audio sources normal
            InitializeAudio();
            InitializeAmbientSound();

            // Thêm khởi tạo AudioSource damage
            InitializeAudio();

            if (healthBar != null)
            {
                healthBar.value = health; // ✅ Cập nhật giá trị máu
            }
            mainCamera = Camera.main.transform; // ✅ Lấy camera chính

            animator = GetComponent<Animator>();
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
        }

        private void Update()
        {
            //Điều kiện dừng sound normal khi bị damage
            if (!died && canPlayAmbientSound)
            {
                // Cập nhật thời gian không nhận damage
                timeWithoutDamage += Time.deltaTime;

                // Kiểm tra nếu đủ thời gian chờ và chưa phát âm thanh nền
                if (timeWithoutDamage >= damageCooldown && ambientSoundCoroutine == null)
                {
                    ambientSoundCoroutine = StartCoroutine(PlayAmbientSound());
                }
            }
        }
        private void UpdateSystem()
        {

            //update hp
            if (healthBar != null)
            {
                healthBar.value = health;
            }

            if (!died && health <= 0)
            {
                OnDeath?.Invoke();
            }

            UpdateUI(1);
        }

        private void LateUpdate()
        {
            if (healthBar != null && healthBar.gameObject.activeSelf)
            {
                // Cập nhật vị trí thanh máu
                healthBar.transform.position = transform.position + new Vector3(0, 2f, 0);

                // Kiểm tra mainCamera trước khi sử dụng
                if (mainCamera != null)
                {
                    Quaternion quaternion = Quaternion.LookRotation(mainCamera.forward);
                    healthBar.transform.rotation = quaternion;
                }
                else
                {
                    // Nếu mainCamera bị null, thử lấy lại hoặc bỏ qua
                    mainCamera = Camera.main?.transform;
                    if (mainCamera != null)
                    {
                        Quaternion quaternion = Quaternion.LookRotation(mainCamera.forward);
                        healthBar.transform.rotation = quaternion;
                    }
                    else
                    {
                        Debug.LogWarning("Main camera is missing in the scene!");
                    }
                }
            }
        }

        public void Heal(float heal)
        {
            health += heal;
        }

        //Audio DAMAGE
        private void InitializeAudio()
        {
            // Tự động thêm AudioSource nếu chưa có
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f; // Âm thanh 3D
                audioSource.playOnAwake = false;
            }
        }

        //Audio NORMAL
        private void InitializeAmbientSound()
        {
            // Tạo AudioSource riêng cho âm thanh nền
            audioSourceAmbient = gameObject.AddComponent<AudioSource>();
            audioSourceAmbient.spatialBlend = 1f; // Âm thanh 3D
            audioSourceAmbient.playOnAwake = false;
            audioSourceAmbient.loop = false; // Tắt loop để tự điều khiển

            // Bắt đầu coroutine phát âm thanh nền
            if (ambientSound != null)
            {
                ambientSoundCoroutine = StartCoroutine(PlayAmbientSound());
            }
        }

        private IEnumerator PlayAmbientSound()
        {
            while (!died && canPlayAmbientSound && ambientSound != null)
            {
                audioSourceAmbient.PlayOneShot(ambientSound, ambientSoundVolume);

                // Chờ đến khi âm thanh kết thúc + delay
                float totalWaitTime = ambientSound.length + ambientSoundDelay;
                yield return new WaitForSeconds(totalWaitTime);
            }
            ambientSoundCoroutine = null; // Reset coroutine khi kết thúc
        }

        private void DoDamage(float damage, Actor killer)
        {
            health -= damage;
            this.killer = killer;
            // Debug.Log($"Current Health: {health}"); // ✅ Kiểm tra giá trị
            // Reset thời gian không nhận damage
            timeWithoutDamage = 0f;

            // Dừng âm thanh nền hiện tại
            if (ambientSoundCoroutine != null)
            {
                StopCoroutine(ambientSoundCoroutine);
                ambientSoundCoroutine = null;
            }
            audioSourceAmbient.Stop();
            StartCoroutine(DamageCooldownRoutine());

            // Phát âm thanh khi nhận damage
            if (damageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(damageSound, soundVolume);
            }
            else
            {
                Debug.LogWarning("Missing damage sound or AudioSource component");
            }


            // Animation handling
            if (health > 0)
            {
                animator.SetTrigger("damage");
            }

            if (type == HealthType.Humanoid && Actor.characterManager != null)
            {
                Actor.characterManager.cameraManager.ShakeCameras(damageCameraShake);
            }

            UpdateSystem();
        }
        private IEnumerator DamageCooldownRoutine()
        {
            canPlayAmbientSound = false;
            yield return new WaitForSeconds(damageCooldown);
            canPlayAmbientSound = true;
        }

        private void UpdateUI(float alpha)
        {
            if (type == HealthType.Humanoid && Actor.characterManager != null)
            {
                UIManager.Instance.DamageIndicator.Show(alpha);
                UIManager.Instance.HealthDisplay.UpdateCard(health, Actor.actorName, true);
            }
        }

        private void Die()
        {
            // Dừng âm thanh nền khi chết
            if (ambientSoundCoroutine != null)
            {
                StopCoroutine(ambientSoundCoroutine);
            }

            if (audioSourceAmbient != null)
            {
                audioSourceAmbient.Stop();
            }

            // Death animation
            animator.SetTrigger("die");

            // Kích hoạt hệ thống nổ
            StartCoroutine(ExplodeRoutine());

            if (type == HealthType.Humanoid)
            {
                if (Actor.actorManager && Actor.actorManager.respawnable) Actor.ConfirmDeath();
            }

            if (destoryOnDeath && !destroyRoot) Destroy(gameObject, destroyDelay);
            if (destoryOnDeath && destroyRoot) Destroy(gameObject.transform.parent.gameObject, destroyDelay);
            if (ragdoll) ragdoll.Enable(deathForce);

            if (!died) Respwan();

            if (deathEffect)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
                effect.SetActive(true);
            }

            died = true;

            // Sinh ra đối tượng thay thế (nếu đã gán trong Inspector)
            if (ammoPrefab != null)
            {
                // Sinh ra prefab tại vị trí và hướng của zombie hiện tại
                Instantiate(ammoPrefab, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("Chưa gán replacementPrefab trong Inspector!");
            }

            // KINH NGHIEM (nếu đã gán trong Inspector)
            if (kinhNghiem != null)
            {
                // Sinh ra prefab tại vị trí và hướng của zombie hiện tại
                Instantiate(kinhNghiem, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("Chưa gán replacementPrefab trong Inspector!");
            }

            //// Hủy đối tượng zombie hiện tại
            //Destroy(gameObject);
        }


        private IEnumerator ExplodeRoutine()
        {
            yield return new WaitForSeconds(explosionDelay);

            if (explosivePrefab)
            {
                // Tạo explosive instance
                Explosive newExplosive = Instantiate(explosivePrefab, transform.position, transform.rotation);

                // Thiết lập thông số
                newExplosive.source = killer; // Hoặc Actor tùy logic game
                newExplosive.transform.localScale = transform.lossyScale;

                // Kế thừa vận tốc
                if (inheritVelocity && TryGetComponent(out Rigidbody rb))
                {
                    newExplosive.GetComponent<Rigidbody>().linearVelocity = rb.linearVelocity;
                }

                // Kích nổ ngay lập tức
                newExplosive.Explode();
            }
        }

        // Trong script zombieAAA
        public void TriggerExplosionNow()
        {
            if (explosivePrefab)
            {
                // Tạo một instance của explosivePrefab tại vị trí của zombie
                Explosive newExplosive = Instantiate(explosivePrefab, transform.position, transform.rotation);

                // (Nếu cần) Thiết lập thông số cho explosive, ví dụ:
                newExplosive.source = Actor; // hoặc gán Actor phù hợp
                newExplosive.transform.localScale = transform.lossyScale;

                // Kích hoạt nổ ngay lập tức
                newExplosive.Explode();
            }
            else
            {
                Debug.LogWarning("Chưa gán explosivePrefab trong ZombieAAA");
            }
        }
        private void Respwan()
        {
            if (type != HealthType.Humanoid && !Actor) return;

            if (Actor.actorManager && Actor.actorManager.respawnable)
                Actor.actorManager.Respwan(Actor.actorManager.SpwanManager.respawnDelay);

            if (Actor.characterManager != null) deathCamera.Enable(Actor, killer);
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
            return health <= 0;
        }

        public Actor GetActor()
        {
            return Actor;
        }

        public int GetGroupsCount()
        {
            return groups != null ? groups.Length : 0;
        }

        public Ragdoll GetRagdoll()
        {
            return ragdoll;
        }

        internal void TriggerExplosive()
        {
            throw new NotImplementedException();
        }
    }
}