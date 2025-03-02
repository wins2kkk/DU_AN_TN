using UnityEngine;
using UnityEngine.UI;
using TMPro; // Sử dụng TextMeshPro cho UI hiện đại

public class GunSelection : MonoBehaviour
{
    public GunData[] guns; // Mảng chứa tất cả súng
    public Image gunImage;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI magazineText;

    private int currentIndex = 0;

    void Start()
    {
        UpdateGunUI();
    }

    public void SetGunIndex(int index)
    {
        currentIndex = index;
        UpdateGunUI();
    }
    public void NextGun()
    {
        currentIndex = (currentIndex + 1) % guns.Length;
        UpdateGunUI();
    }

    public void PreviousGun()
    {
        currentIndex = (currentIndex - 1 + guns.Length) % guns.Length;
        UpdateGunUI();
    }

    void UpdateGunUI()
    {

        if (guns == null || guns.Length == 0)
        {
            Debug.LogError("GunSelection: Mảng guns trống hoặc chưa được gán!");
            return;
        }

        if (currentIndex < 0 || currentIndex >= guns.Length)
        {
            Debug.LogError("GunSelection: currentIndex ngoài phạm vi! Giá trị: " + currentIndex);
            return;
        }

        GunData gun = guns[currentIndex];

        if (gun == null)
        {
            Debug.LogError("GunSelection: GunData tại currentIndex là null!");
            return;
        }

        Debug.Log("Đang hiển thị súng: " + gun.gunName);
       // gunImage.sprite = gun.gunImage;
        gunNameText.text = "Tên: " + gun.gunName;
        damageText.text = "Dame: " + gun.damage;
        fireRateText.text = "Tốc độ bắn: " + gun.fireRate;
        magazineText.text = "Loại: " + gun.magazineSize;
    }

}
