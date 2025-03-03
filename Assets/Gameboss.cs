using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameboss : MonoBehaviour
{
    public List<GameObject> enemies; // Danh sách quái vật kéo vào từ Inspector
    public GameObject boss; // Boss cần kích hoạt
    public GameObject bossPanel; // Panel thông báo boss xuất hiện

    private void Start()
    {
        if (boss != null)
        {
            boss.SetActive(false); // Ẩn boss khi bắt đầu game
        }

        if (bossPanel != null)
        {
            bossPanel.SetActive(false); // Ẩn panel thông báo boss
        }
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }

        if (enemies.Count == 0)
        {
            StartCoroutine(SpawnBossCoroutine());
        }
    }

    private IEnumerator SpawnBossCoroutine()
    {
        bossPanel.SetActive(true);
        yield return new WaitForSeconds(2f); // Hiển thị thông báo 2 giây
        bossPanel.SetActive(false);

        if (boss != null)
        {
            boss.SetActive(true); // Hiện boss lên
        }
    }
}
