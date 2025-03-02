using UnityEngine;

[CreateAssetMenu (fileName =" New map", menuName ="Scriptable Objects/Maps")]


public class map : ScriptableObject
{
    public int mapIndex;
    public string mapName;
    public string mapDescription;
    public Color nameColor;
    public Sprite mapImage;
    public Object SceneToLoad;


}

