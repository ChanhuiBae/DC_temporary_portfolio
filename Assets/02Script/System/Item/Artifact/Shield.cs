using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private CapsuleCollider2D col;
    private Animator animator;
    private int H_Explode = Animator.StringToHash("Explode");
    private void Awake()
    {
        if(!TryGetComponent<CapsuleCollider2D>(out col))
        {
            Debug.Log("Shield - Awake - CapsuleCollider2D");
        }
        if(!TryGetComponent<Animator>(out animator))
        {
            Debug.Log("Shield - Awake - Animator");
        }
    }
    private void Start()
    {
        GameManager.Inst.artifactManager.shield = this;   
        gameObject.SetActive(false);
    }

    public void SetShield()
    {
        GameManager.Inst.artifactManager.shieldActive = true;
        animator.SetBool(H_Explode, false);
        GameManager.Inst.player.AddShield(2, col);
        GameManager.Inst.player.SetShield(2, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Skill")
        {
            animator.SetBool(H_Explode, true);
            StartCoroutine(WaitDelete());
            collision.gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitDelete()
    {
        GameManager.Inst.player.SetShield(2, false);
        GameManager.Inst.artifactManager.shieldActive = false;
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}
