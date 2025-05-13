using UnityEditor;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] bool _OnlyOnStart;

    public void LookAtCamera()
    {
        Vector3 targetDir =  transform.position - Camera.main.transform.position;
        //Vector3 targetDir = Vector3.ProjectOnPlane(Camera.main.transform.position - transform.position, Camera.main.transform.forward);
        transform.rotation = Quaternion.LookRotation(targetDir, Camera.main.transform.up);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LookAtCamera();
        enabled = !_OnlyOnStart;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtCamera();
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(FaceCamera))]
class FaceCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Look at camera")) ((FaceCamera)target).LookAtCamera();
    }
}
#endif