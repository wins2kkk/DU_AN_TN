using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Akila.FPSFramework;

public class PlayerWin : MonoBehaviour
{
    public GameObject winPanel; // Kéo Panel Win từ Inspector vào đây
    private CanvasGroup canvasGroup;

  //  public string nextSceneName = "NextScene"; // Tên scene tiếp theo
    public float delayBeforeNextScene = 5f; // Thời gian chờ trước khi chuyển scene

    private void Start()
    {
        if (winPanel != null)
        {
            canvasGroup = winPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = winPanel.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0; // Bắt đầu với panel ẩn
            winPanel.SetActive(false);

            Debug.Log($"[Start] ReachedIndex: {PlayerPrefs.GetInt("ReachedIndex", 0)}");
        }

    }

    void UnlockNewLevel()
    {
        Debug.Log("🔵 UnlockNewLevel() was called!");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int reachedIndex = PlayerPrefs.GetInt("ReachedIndex", 0);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        Debug.Log($"🔹 Current Scene Index: {currentSceneIndex}");
        Debug.Log($"🔹 Reached Index Before: {reachedIndex}");
        Debug.Log($"🔹 Unlocked Level Before: {unlockedLevel}");

        if (currentSceneIndex >= reachedIndex)
        {
            int newReachedIndex = currentSceneIndex + 1;
            PlayerPrefs.SetInt("ReachedIndex", newReachedIndex);

            if (newReachedIndex > unlockedLevel)
            {
                PlayerPrefs.SetInt("UnlockedLevel", newReachedIndex);
            }

            PlayerPrefs.Save();

            Debug.Log($"🟢 New ReachedIndex: {PlayerPrefs.GetInt("ReachedIndex")}");
            Debug.Log($"🟢 New UnlockedLevel: {PlayerPrefs.GetInt("UnlockedLevel")}");
        }

        // 🟢 Gọi UnlockNextLevel từ LevelButton
        GameObject levelButtonObj = GameObject.Find("LevelButton_" + (currentSceneIndex + 1));
        if (levelButtonObj != null)
        {
            LevelButton levelButton = levelButtonObj.GetComponent<LevelButton>();
            if (levelButton != null)
            {
                levelButton.UnlockNextLevel("LEVEL_" + (currentSceneIndex + 1));
                Debug.Log("🟢 Next level unlocked: LEVEL_" + (currentSceneIndex + 1));
            }
        }
    




}
//private void Awake()
//{
//    if (!LoadingScreen.Instance)
//        SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
//}


//public void Restart(string name)
//{
//    Time.timeScale = 1; // Đảm bảo game không bị pause khi chuyển scene
//    StartCoroutine(LoadingScreen.Instance.LoadSceneAsync(name));
//    //Debug.Log("cc");
//    //FindObjectOfType<HealthSystem>().ResetPlayerData();
//    //PlayerPrefs.SetFloat("PlayerTime", 0);
//    //PlayerPrefs.SetInt("Restarted", 1);
//    //PlayerPrefs.SetInt("PlayerCoins", 0);
//    //PlayerPrefs.Save();

//    SceneManager.LoadScene("tvtscene");
//}

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                StartCoroutine(FadeInPanel());
            }

            UnlockNewLevel();

            Time.timeScale = 0;
            EventSystem.current.enabled = false; // Vô hiệu hóa input
          //  StartCoroutine(LoadNextSceneAfterDelay()); // Chuyển scene sau 5 giây
        }
    }

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

    //private IEnumerator LoadNextSceneAfterDelay()
    //{
    //    yield return new WaitForSecondsRealtime(delayBeforeNextScene); // Chờ 5 giây

    //    // 🟢 Reset dữ liệu trước khi load scene thắng
    //    PlayerPrefs.SetFloat("PlayerTime", 0); // Reset thời gian chơi
    //    PlayerPrefs.SetInt("PlayerCoins", 0);  // Reset coin nếu cần
    //    PlayerPrefs.SetInt("Restarted", 1);    // Đánh dấu restart để kiểm tra sau khi load scene
    //    PlayerPrefs.Save();

    //    Time.timeScale = 1; // Đảm bảo game tiếp tục khi chuyển scene
    //   // SceneManager.LoadScene(nextSceneName);
    //}

}