using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class fpsCounter : MonoBehaviour
{
    TMP_Text txt;

    Queue<float> _frameTimes = new();

    private void Awake()
    {
        TryGetComponent(out txt);
        InvokeRepeating("ComputeAverageFPS", 0,.1f);
    }



    void ComputeAverageFPS()
    {
        _frameTimes.Enqueue(Time.deltaTime);
        if (_frameTimes.Count > 10) _frameTimes.Dequeue();

        float sum = 0;
        foreach (float f in _frameTimes)
        {
            sum += f;
        }
        sum /= _frameTimes.Count;
        txt.text = Mathf.RoundToInt(1f / sum).ToString();
    }
}
