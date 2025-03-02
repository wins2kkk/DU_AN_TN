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

    private float countdown = 0f;
    private int currentWaveIndex = 0;

    void Start()
    {
        waveTimerSlider.minValue = 0f;
        waveTimerSlider.maxValue = 1f;
        waveTimerSlider.value = 0f;

        // Bắt đầu spawn đợt 1
        StartCoroutine(SpawnWave(0));
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Length) return;

        // Cập nhật slider theo thời gian
        countdown += Time.deltaTime;
        waveTimerSlider.value = countdown / (waveInterval * 2); // Tổng 100s cho cả slider

        // Spawn đợt 2 khi được 50% (50s)
        if (countdown >= waveInterval && currentWaveIndex == 1)
        {
            StartCoroutine(SpawnWave(1));
        }

        // Spawn đợt 3 khi slider full (100s)
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
}
