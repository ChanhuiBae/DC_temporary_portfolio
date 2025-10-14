using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuffType
{
    None,
    HighSpeedCasting,
    Stead,
    RedMoon,
}
public class Buff : MonoBehaviour
{
    protected int maxStack;
    protected Dictionary<CharacterController,int> stack = new Dictionary<CharacterController, int>();
    public int GetCount()
    {
        return stack.Count;
    }
    protected float fullTime;
    protected float timedelay;

    protected Dictionary<CharacterController, Coroutine> coroutines = new Dictionary<CharacterController, Coroutine>();

    protected virtual void Awake()
    {
        maxStack = 1;
        fullTime = 1;
        timedelay = 1;
    }

    public virtual void StartBuff(bool use, CharacterController character)
    {
        if (coroutines.ContainsKey(character))
        {
            if (use)
            {
                if (coroutines[character] != null)
                    StopCoroutine(coroutines[character]);
                coroutines[character] = StartCoroutine(PlayBuff(character));
            }
            else
            {
                if (coroutines[character] != null)
                    StopCoroutine(coroutines[character]);
                stack[character] = 0;
                coroutines.Remove(character);
                stack.Remove(character);
            }
        }
        else
        {
            if (use)
            {
                stack.Add(character, 0);
                coroutines.Add(character, StartCoroutine(PlayBuff(character)));
            }
        }
    }

    protected virtual IEnumerator PlayBuff(CharacterController character)
    {
        stack[character]++;
        if(stack[character] > maxStack)
            stack[character] = maxStack;
        float time = Time.time;
        while (Time.time < time + fullTime)
        {
            // do buff.
            yield return YieldInstructionCache.WaitForSeconds(timedelay);
        }
        stack[character] = 0;
    }

}
