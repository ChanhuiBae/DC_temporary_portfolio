using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public enum CharacterStateDC
{
    Idle,
    Ready,
    Run,
    Attack,
    Slash,
    Jab,
    Push,
    TakeDamage,
    Die
}

public class CharacterController : MonoBehaviour
{
    protected CharacterAnimationController anim;
    private SpriteRenderer render;
    private SpriteLibrary spriteLibrary;

    protected bool isAlive;
    public bool IsAlive
    {
        set => isAlive = value;
        get => isAlive;
    }
    protected CharacterStateDC state;
    public CharacterStateDC State
    {
        set => state = value;
        get => state;
    }

    private Direction look;
    public Direction Look
    {
        set
        {
            look = value;
            if (value == Direction.Left)
                render.flipX = true;
            else
                render.flipX = false;
        }
        get => look;
    }

    protected SortedDictionary<int, Collider2D> shield = new SortedDictionary<int, Collider2D>();
    protected CapsuleCollider2D col;
    public bool GetShield()
    {
        if(col == null)
            return false;
        return !col.enabled;
    }

    public void AddShield(int order, Collider2D col)
    {
        if (!shield.ContainsKey(order))
        {
            shield.Add(order, col);
            shield[order].enabled = true;
        }
    }
    public void SetShield(int order, bool use)
    {
        if (shield.ContainsKey(order))
            shield[order].enabled = use;
        if (use)
        {
            foreach (var pair in shield)
            {
                if (pair.Key == order)
                    break;
                else
                    shield[pair.Key].enabled = false;
            }
        }
        else
        {
            int preOrder = 0;
            foreach (var pair in shield)
            {
                if (pair.Key == order)
                    break;
                else
                    preOrder = pair.Key;
            }
            SetShield(preOrder, true);
        }
    }

    protected virtual void Awake()
    {
        if(!TryGetComponent<CharacterAnimationController>(out anim))
        {
            Debug.Log("CharacterContorller - Awake - AnimationController");
        }    
        if(!transform.GetChild(0).TryGetComponent<SpriteRenderer>(out render))
        {
            Debug.Log("CharacterController - Awkae - SpriteRenderer");
        }
        if (!transform.GetChild(0).TryGetComponent<SpriteLibrary>(out spriteLibrary))
        {
            Debug.Log("CharacterController - Awkae - SpriteLibrary");
        }
        if(!TryGetComponent<CapsuleCollider2D>(out col))
        {
            Debug.Log("CharacterController - Awake - CapsuleCollider2D");
        }
        else
        {
            AddShield(0, col);
        }
        state = CharacterStateDC.Idle;
    }

    public void SetSprite(string name)
    {
        spriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("SpriteLibrary/" + name);
    }

    public void PlayAnimation(CharacterStateDC state)
    {
        if (this.state != state)
        {
            this.state = state;
            switch (state)
            {
                case CharacterStateDC.Idle:
                    anim.SetRun(false);
                    anim.SetReady(false);
                    break;
                case CharacterStateDC.Ready:
                    anim.SetRun(false);
                    anim.SetReady(true);
                    break;
                case CharacterStateDC.Run:
                    anim.SetRun(true);
                    break;
                case CharacterStateDC.Attack:
                    anim.Attack();
                    break;
                case CharacterStateDC.Slash:
                    anim.Slash();
                    break;
                case CharacterStateDC.Jab:
                    anim.Jab();
                    break;
                case CharacterStateDC.Push:
                    anim.Push();
                    break;
                case CharacterStateDC.TakeDamage:
                    anim.TakeDamage();
                    break;
                case CharacterStateDC.Die:
                    anim.Die();
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case CharacterStateDC.Attack:
                    anim.Attack();
                    break;
                case CharacterStateDC.Slash:
                    anim.Slash();
                    break;
                case CharacterStateDC.Jab:
                    anim.Jab();
                    break;
                case CharacterStateDC.Push:
                    anim.Push();
                    break;
                case CharacterStateDC.TakeDamage:
                    anim.TakeDamage();
                    break;
            }
        }
    }

    public virtual void TakeDamage(float damage, AttributeType attributeType, bool critical)
    {

    }
}
