using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Health System")]
    public class HealthSystemquan : MonoBehaviour, IDamageable
    {
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
        [SerializeField] private string sfxGroup = "SFX"; // 🔹 Gán vào nhóm SFX trong AudioMixer
        private AudioSource audioSource;



        public HealthTypeQuan type = HealthTypeQuan.Other;
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

        //trong public class  [Header("Drop KinhNghiem")]
        public GameObject kinhNghiem; // 

        private void Start()
        {
            // Khởi tạo audio sources normal
            InitializeAudio();
            InitializeAmbientSound();

            // Thêm khởi tạo AudioSource damage
            InitializeAudio();



            Actor = GetComponent<Actor>();
            ragdoll = GetComponent<Ragdoll>();
            animator = GetComponent<Animator>();

            OnDeath.AddListener(Die);
            if (FindObjectOfType<GameManager>()) deathCamera = FindObjectOfType<GameManager>().DeathCamera;

            MaxHealth = health;

            if (type == HealthTypeQuan.Humanoid)
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

            if (type == HealthTypeQuan.Other)
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

        private void UpdateSystem()
        {
            if (!died && health <= 0)
            {
                OnDeath?.Invoke();
            }

            UpdateUI(1);
        }

        public void Heal(float heal)
        {
            health += heal;
        }

        private void DoDamage(float damage, Actor killer)
        {
            health -= damage;
            this.killer = killer;

            if (health > 0 && animator != null)  // Kiểm tra animator trước khi gọi
            {
                animator.SetTrigger("DAMAGE");
            }
            else if (animator == null)
            {
                Debug.LogWarning("Animator không được gán trên " + gameObject.name);
            }

            if (type == HealthTypeQuan.Humanoid && Actor.characterManager != null)
            {
                Actor.characterManager.cameraManager.ShakeCameras(damageCameraShake);
            }

            // Dừng âm thanh nền hiện tại
            if (ambientSoundCoroutine != null)
            {
                StopCoroutine(ambientSoundCoroutine);
                ambientSoundCoroutine = null;
            }
            audioSourceAmbient.Stop();
            StartCoroutine(DamageCooldownRoutine());

            // Phát âm thanh khi nhận damage
         /*   if (damageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(damageSound, soundVolume);
            }
            else
            {
                Debug.LogWarning("Missing damage sound or AudioSource component");
            }*/


            // ❌ Không dùng AudioSource, thay vào đó gửi về AudioManager
            if (damageSound != null)
            {
                QAudioManager.Instance.PlaySound(damageSound, soundVolume, sfxGroup);
            }

            if (ambientSoundCoroutine != null)
            {
                StopCoroutine(ambientSoundCoroutine);
                ambientSoundCoroutine = null;
            }

            StartCoroutine(DamageCooldownRoutine());

            if (health <= 0) OnDeath?.Invoke();


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
            if (type == HealthTypeQuan.Humanoid && Actor.characterManager != null)
            {
                UIManager.Instance.DamageIndicator.Show(alpha);
                UIManager.Instance.HealthDisplay.UpdateCard(health, Actor.actorName, true);
            }
        }

        private void Die()
        {
            //void Die  // KINH NGHIEM (nếu đã gán trong Inspector)
            if (kinhNghiem != null)
            {
                // Sinh ra prefab tại vị trí và hướng của zombie hiện tại
                Instantiate(kinhNghiem, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("Chưa gán replacementPrefab trong Inspector!");
            }


            animator.SetTrigger("die");

            if (type == HealthTypeQuan.Humanoid)
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

        }

        private void Respwan()
        {
            if (type != HealthTypeQuan.Humanoid && !Actor) return;

            if (Actor.actorManager && Actor.actorManager.respawnable)
                Actor.actorManager.Respwan(Actor.actorManager.SpwanManager.respawnDelay);

            //if player enable death cam
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
    }

    public enum HealthTypeQuan
    {
        Humanoid = 0,
        Other = 1
    }


}