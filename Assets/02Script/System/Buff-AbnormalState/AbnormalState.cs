using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbnormalStateType
{
    None,
    Burn,
    Bleeding,
    Poisoning,
}

public class AbnormalState : MonoBehaviour
{
    protected int maxStack;
    protected Dictionary<CharacterController,int> stack = new Dictionary<CharacterController, int>();
    public int GetCount()
    {
        return stack.Count;
    }
    protected float fullTime;
    protected float timedelay;

    protected Dictionary<CharacterController,Coroutine> coroutines = new Dictionary<CharacterController,Coroutine>();

    protected virtual void Awake()
    {
        maxStack = 1;
        fullTime = 1;
        timedelay = 1;
    }

    public void StartAbnormalState(CharacterController character, float atk)
    {
        if (coroutines.ContainsKey(character))
        {
            StopCoroutine(coroutines[character]); 
            coroutines[character] = StartCoroutine(PlayAbnormalState(character, atk));
        }
        else
        {
            stack.Add(character, 0);
            coroutines.Add(character, StartCoroutine(PlayAbnormalState(character,atk)));
        }
    }

    protected virtual IEnumerator PlayAbnormalState(CharacterController character, float atk)
    {
        stack[character]++;
        if (stack[character] > maxStack)
            stack[character] = maxStack;
        float time = Time.time;
        while(Time.time < time + fullTime)
        {
            // do abnormal state effect to character.
            yield return YieldInstructionCache.WaitForSeconds(timedelay);
        }
        stack[character] = 0;
    }
}
