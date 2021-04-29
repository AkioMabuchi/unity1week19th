using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerSizeFixer : MonoBehaviour
{
    private CanvasScaler _canvasScaler;
    // Start is called before the first frame update
    void Start()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        _canvasScaler.matchWidthOrHeight = Screen.width * 9 / Screen.height >= 16 ? 1.0f : 0.0f;
    }
}
