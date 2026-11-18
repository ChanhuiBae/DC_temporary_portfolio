using System.Collections;
using UnityEngine;

public class Burn : AbnormalState
{
    protected override void Awake()
    {
        maxStack = 6;
        fullTime = 3;
        timedelay = 1;
    }

    protected override IEnumerator PlayAbnormalState(CharacterController character, float atk)
    {
        // 중첩을 1 증가시키되 최대 6중첩으로 제한
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

        stack[character] = 0;
    }
}