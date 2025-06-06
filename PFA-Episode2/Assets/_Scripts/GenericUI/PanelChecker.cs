using UnityEngine;

public class PanelChecker : MonoBehaviour
{
    private void Update()
    {
        if (IsElementPartiallyOffScreen()) { print("C'est en dehors"); }
        else { print("c'est dans l'écran"); }
    }

    private bool IsElementPartiallyOffScreen()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.localPosition);
        print(pos); // (-24390, 13338)
        //print(Screen.width); //2340
        //print(Screen.height); //1080
        bool isOffscreen = pos.x <= 0 || pos.x >= Screen.width ||pos.y <= 0 || pos.y >= Screen.height;
        return isOffscreen;
    }
}
