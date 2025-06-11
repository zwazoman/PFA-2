using UnityEngine;

public class ScreenShotMaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int i = PlayerPrefs.GetInt("screenshotID", 0) ;
        
            string filename = "highResScreenShot_" + i.ToString() + ".png";
            ScreenCapture.CaptureScreenshot(filename, 2);
            print("Saved screen shot : " + filename);

            PlayerPrefs.SetInt("screenshotID", i + 1);
        }
    }
}
