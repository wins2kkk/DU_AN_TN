using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelManager : MonoBehaviour
{
    public GameObject deathPanel;

    private void Start()
    {
        if (deathPanel == null)
        {
            Debug.LogWarning("⚠️ DeathPanel chưa được gán trong Inspector!");
        }
        else
        {
            deathPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked; // 🔒 Khóa chuột ban đầu
            Cursor.visible = false; // Ẩn chuột ban đầu
        }
    }

    public void ShowDeathPanel()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //if (FindObjectOfType<CharacterInput>() != null)
            //{
            //    FindObjectOfType<CharacterInput>().controls.Player.Disable();
            //    Debug.Log("🚫 Input đã bị tắt khi chết.");
            //}
        }
    }


    public void HideDeathPanel()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked; // 🔒 Khóa lại chuột
            Cursor.visible = false; // Ẩn chuột lại
            Debug.Log("🔄 Death Panel đã ẩn.");
        }
    }
}
