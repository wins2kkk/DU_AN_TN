using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public int damage; 
    public float fireRate;// tốc độ bắn
    public int magazineSize; //loại đạn
    public Sprite gunImage;
}
