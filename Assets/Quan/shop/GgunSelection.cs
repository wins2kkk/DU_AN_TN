using UnityEngine;

public class GgunSelection : MonoBehaviour
{
    public int currentGunIndex;
    public GameObject[] guns;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGunIndex = PlayerPrefs.GetInt("SelectedGun", 0);
        foreach (GameObject gun in guns)
            gun.SetActive(false);

        guns[currentGunIndex].SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
