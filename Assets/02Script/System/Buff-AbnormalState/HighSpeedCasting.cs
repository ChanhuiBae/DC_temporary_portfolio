using System.Collections;
using UnityEngine;

public class HighSpeedCasting : Buff
{
    private float castSpeedPerStack = 0.05f;

    protected override void Awake()
    {
        maxStack = 10;
    }
    public override void StartBuff(bool use, CharacterController character)
    {
        if (coroutines.TryGetValue(character, out Coroutine coroutine))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        if (use)
        {
            if (!stack.ContainsKey(character))
                stack.Add(character, 0);

            coroutines[character] = StartCoroutine(PlayBuff(character));
        }
        else
        {
            RemoveSpeedBonus(character);
            coroutines.Remove(character);
            stack.Remove(character);
        }
    }

    private void RemoveSpeedBonus(CharacterController character)
    {
        if (stack.ContainsKey(character))
        {
            int currentStack = stack[character];

            GameManager.Inst.Exploration.cast_speed -= currentStack * castSpeedPerStack;

            stack[character] = 0;
        }
    }

    protected override IEnumerator PlayBuff(CharacterController character)
    {
        int previousStack = stack[character];
        int newStack = Mathf.Min(previousStack + 1, maxStack);

        stack[character] = newStack;

        // 실제로 늘어난 중첩만큼만 속도 증가
        GameManager.Inst.Exploration.cast_speed
            += (newStack - previousStack) * castSpeedPerStack;

        while (GameManager.Inst.Exploration.SkillQueueCount > 0
            || GameManager.Inst.Exploration.UsingSkill)
        {
            yield return null;
        }

        RemoveSpeedBonus(character);
    }
}
