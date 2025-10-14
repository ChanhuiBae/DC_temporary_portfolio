using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimationController : MonoBehaviour
{
    private Animator animator;
    private int H_Start = Animator.StringToHash("Start");
    private int H_Type = Animator.StringToHash("Type");
    private void Awake()
    {
        if(!transform.TryGetComponent<Animator>(out animator))
        {
            Debug.Log("ChestAnimationController - Awake - Animator");
        }
    }

    public void SetType(int type)
    {
        animator.SetInteger(H_Type, type);
    }

}
