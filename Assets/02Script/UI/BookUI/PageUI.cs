using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageUI : MonoBehaviour
{
    public void Close()
    {
        transform.LeanRotateY(90, 0.5f).setEaseInSine();
    }

    public void Open()
    {
        transform.LeanRotateY(0, 0.5f).setEaseOutSine();
    }

    public virtual void Show()
    {
        transform.LeanRotateY(0, 0);
    }
}
