using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour, UIManager
{
    private Image left;
    private Image right;
    private Image rightUp;
    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Image>(out left))
        {
            Debug.Log("Fade - Awake - Image");
        }
        if (!transform.GetChild(1).TryGetComponent<Image>(out right))
        {
            Debug.Log("Fade - Awake - Image");
        }
        GameObject ru = GameObject.Find("RightUp");
        if (!ru.TryGetComponent<Image>(out rightUp))
        {
            Debug.Log("Fade - Awake - Image");
        }
    }

    public void FadeIn(float time)
    {
        left.transform.LeanScaleX(1, time);
        right.transform.LeanScaleX(1, time);
        rightUp.transform.LeanScaleY(1, time);
    }

    public void FadeOut(float time)
    {
        left.transform.LeanScaleX(0, time);
        right.transform.LeanScaleX(0, time);
        rightUp.transform.LeanScaleY(0, time);
    }

}
