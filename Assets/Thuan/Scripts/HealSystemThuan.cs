using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;



namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Health System")]
    public class HealthSystemThuan : MonoBehaviour, IDamageable
    {//trong public class  [Header("Drop KinhNghiem")]
        public GameObject kinhNghiem; // Prefab sẽ được sinh ra khi zombie chết

        public HealthType type = HealthType.Other;
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
        private bool died = false;
        public bool deadConfirmed { get; set; }

        public Animator animator;
        public GameObject enemyPrefab;       // Prefab của enemy mới
        public int spawnCount = 3;           // Số lượng enemy mới cần sinh ra
        public float spawnRadius = 2f;       // Bán kính sinh enemy quanh vị trí hiện tại
        public float spawnDelay = 3f;        // Thời gian trì hoãn trước khi sinh enemy


        private void Start()
        {
            Actor = GetComponent<Actor>();
            ragdoll = GetComponent<Ragdoll>();
            animator = GetComponent<Animator>();

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
            if(health > 0)
            {
                animator.SetTrigger("damage");
                AudioManager.instance.Play("dam");
            }
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

        private void Die()
        {
            if (HasParameter(animator, "die"))
            {
                animator.SetTrigger("die");
                AudioManager.instance.Play("dead");
            }
            else
            {
                Debug.LogWarning("Parameter 'die' not found in Animator.");
            }


            if (kinhNghiem != null)
            {
                // Sinh ra prefab tại vị trí và hướng của zombie hiện tại
                Instantiate(kinhNghiem, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("Chưa gán replacementPrefab trong Inspector!");
            }
            if (type == HealthType.Humanoid)
            {
                if (Actor.actorManager && Actor.actorManager.respawnable) Actor.ConfirmDeath();
            }

            if (destoryOnDeath && !destroyRoot) Destroy(gameObject, destroyDelay);
            if (destoryOnDeath && destroyRoot) Destroy(gameObject.transform.parent.gameObject, destroyDelay);
            if (ragdoll) ragdoll.Enable(deathForce);

            if (!died)
            {
                Respwan();                      // Gọi Respawn nếu chưa chết
                StartCoroutine(SpawnEnemiesAfterDelay());  // Gọi Coroutine để spawn enemy sau 3 giây
               
            }

            if (deathEffect)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
                effect.SetActive(true);
            }

            died = true;
            Destroy(gameObject, spawnDelay + 1f);  // Hủy đối tượng sau khi hoàn thành
        }

        private IEnumerator SpawnEnemiesAfterDelay()
        {
            yield return new WaitForSeconds(3f);

            int spawnCount = 3;
            float spawnRadius = 5f;  // Bán kính spawn là 5 đơn vị

            for (int i = 0; i < spawnCount; i++)
            {
                // Tính toán vị trí ngẫu nhiên trong bán kính 5 đơn vị quanh enemy
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius),  // X ngẫu nhiên trong khoảng [-5, 5]
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius),  // Y ngẫu nhiên trong khoảng [-5, 5]
                    0f  // Z giữ nguyên ở 0 nếu bạn làm game 2D
                );

                Vector3 spawnPosition = transform.position + randomOffset;
               // Debug.Log("Spawning enemy at: " + spawnPosition);

                if (enemyPrefab != null)
                {
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }
            }

            Debug.Log("All enemies spawned!");
           // Destroy(gameObject);  // Xóa enemy sau khi spawn xong
        }




        private bool HasParameter(Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                {
                    return true;
                }
            }
            return false;
        }

        private void Respwan()
        {
            if (type != HealthType.Humanoid && !Actor) return;

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

    public enum HealthTypee
    {
        Humanoid = 0,
        Other = 1
    }
}