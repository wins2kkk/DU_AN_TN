using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public EnemyType[] enemies;
        public float spawnRadius = 20f; // Phạm vi spawn quanh điểm spawn
    }

    public Wave[] waves;               // Các đợt spawn
    public Transform[] spawnPoints;    // Điểm spawn
    public Slider waveTimerSlider;     // Slider hiển thị
    public float waveInterval = 50f;   // Thời gian giữa các đợt

    public GameObject bossPrefab;       // Prefab của boss
    public Transform bossSpawnPoint;    // Điểm spawn của boss

    private float countdown = 0f;
    private int currentWaveIndex = 0;
    private int killCount = 0;          // Số kill hiện tại
    private bool bossSpawned = false;   // Kiểm tra boss đã spawn chưa

    void Start()
    {
        waveTimerSlider.minValue = 0f;
        waveTimerSlider.maxValue = 1f;
        waveTimerSlider.value = 0f;

        StartCoroutine(SpawnWave(0));
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Length) return;

        // Cập nhật slider theo thời gian
        countdown += Time.deltaTime;
        waveTimerSlider.value = countdown / (waveInterval * 2); // Tổng 100s cho cả slider

        // Spawn wave 2 khi đạt 50s
        if (countdown >= waveInterval && currentWaveIndex == 1)
        {
            StartCoroutine(SpawnWave(1));
        }

        // Spawn wave 3 khi đạt 100s
        if (countdown >= waveInterval * 2 && currentWaveIndex == 2)
        {
            StartCoroutine(SpawnWave(2));
        }
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        if (waveIndex >= waves.Length) yield break;

        Wave wave = waves[waveIndex];
        Debug.Log($"Bắt đầu {wave.waveName}");

        foreach (EnemyType enemy in wave.enemies)
        {
            for (int i = 0; i < enemy.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 randomPos = spawnPoint.position + Random.insideUnitSphere * wave.spawnRadius;
                randomPos.y = spawnPoint.position.y;
                Instantiate(enemy.enemyPrefab, randomPos, Quaternion.identity);
            }
        }

        currentWaveIndex++; // Chuyển sang đợt tiếp theo
        yield return null;
    }

    public void EnemyDefeated()
    {
        Debug.Log("EnemyDefeated() được gọi!"); // Kiểm tra xem có vào hàm không
        killCount++;
        Debug.Log($"Kill count: {killCount}");

        if (killCount >= 30 && !bossSpawned)
        {
            StartCoroutine(SpawnBoss());
        }
    }


    IEnumerator SpawnBoss()
    {
        bossSpawned = true;
        Debug.Log("Boss xuất hiện!");
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        yield return null;
    }
}
