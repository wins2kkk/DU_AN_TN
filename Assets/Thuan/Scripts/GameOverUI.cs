using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI timeText;

    private void Start()
    {
        // Lấy dữ liệu từ PlayerPrefs
        int playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        int playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        float playTime = PlayerPrefs.GetFloat("PlayerTime", 0f);

        // Tính toán phút và giây từ thời gian chơi
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        // Cập nhật UI
        if (levelText != null) levelText.text = $"LEVEL: {playerLevel}";
        if (coinText != null) coinText.text = $"RUBY: {playerCoins}";
        if (timeText != null) timeText.text = $"TIME: {minutes:00}:{seconds:00}";
    }
}
