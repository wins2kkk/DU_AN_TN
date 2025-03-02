using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int currentGunIndex = 0;
    public GameObject[] gunModels;

    public GunSelection gunSelection; // Thêm biến tham chiếu tới GunSelection


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGunIndex = PlayerPrefs.GetInt("SelectedGun", 0);
        foreach (GameObject gun in gunModels) 
            gun.SetActive(false);

        gunModels[currentGunIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeNext()
    {
        gunModels[currentGunIndex].SetActive(false);

        currentGunIndex++;
        if (currentGunIndex == gunModels.Length)
            currentGunIndex = 0;

        gunModels[currentGunIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedGun", currentGunIndex);

        gunSelection.SetGunIndex(currentGunIndex);

    }

    public void ChangePrevious()
    {
        gunModels[currentGunIndex].SetActive(false);
        
        currentGunIndex--;
        if(currentGunIndex == -1)
            currentGunIndex = gunModels.Length - 1;

        gunModels[currentGunIndex].SetActive(true);
        PlayerPrefs.SetInt("SelectedGun", currentGunIndex);

        // Gọi cập nhật UI theo súng hiện tại
        gunSelection.SetGunIndex(currentGunIndex);

    }
}
