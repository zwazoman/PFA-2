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
        Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        print(pos);
        print(Screen.width);
        print(Screen.height);
        bool isOffscreen = pos.x <= 0 || pos.x >= Screen.width ||pos.y <= 0 || pos.y >= Screen.height;
        return isOffscreen;
    }
}
