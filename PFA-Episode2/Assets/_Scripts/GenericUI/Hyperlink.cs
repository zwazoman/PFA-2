using UnityEngine;

public class Hyperlink : MonoBehaviour
{
    public string link;
    public void Open()
    {
        Application.OpenURL(link);
    }
}
