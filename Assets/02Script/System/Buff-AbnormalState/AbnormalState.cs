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
    protected float fullTime;
    protected float timedelay;

    public int GetCount()
    {
        int count = 0;

        foreach (var pair in stack)
        {
            CharacterController character = pair.Key;
            int currentStack = pair.Value;

            // 파괴되었거나 사망한 대상은 제외
            if (character == null || !character.IsAlive)
                continue;

            // 중첩이 남아 있는 대상만 집계
            if (currentStack > 0)
                count++;
        }

        return count;
    }

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
        stack[character] = Mathf.Min(stack[character] + 1, maxStack);

        float time = Time.time;

        while (Time.time < time + fullTime)
        {
            character.TakeDamage(
                0.05f * atk * stack[character],
                AttributeType.Fire,
                false);

            yield return YieldInstructionCache.WaitForSeconds(timedelay);
        }

        // 상태이상이 종료되면 집계 대상에서 제외됨
        stack[character] = 0;
    }
}
