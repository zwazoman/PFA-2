using UnityEngine;

[CreateAssetMenu(fileName = "new color", menuName = "UI/ColorPanel")]
public class ColorPanel : ScriptableObject
{
    public Color32 ColorLight;
    public Color32 ColorMid;
    public Color32 ColorDark;
}
