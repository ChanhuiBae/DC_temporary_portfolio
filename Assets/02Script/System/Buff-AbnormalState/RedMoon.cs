using System.Collections;
using UnityEngine;

public class RedMoon : Buff, IListener
{
    private FightSceneManager manager;
    private CharacterController player;
    private int condition;
    private int stackCount;
    protected override void Awake()
    {
        maxStack = 3;
        condition = -1;
        stackCount = 0;
    }

    private void Start()
    {
        GameManager.Inst.eventManager.AddListener(GameEventType.MonsterTakeDamage, this);
        GameManager.Inst.eventManager.AddListener(GameEventType.MonsterDie, this);
    }
    public override void StartBuff(bool use, CharacterController character)
    {
        if (use)
        {
            if (!stack.ContainsKey(character))
            {
                stack.Add(character, stackCount);
            }
            player = character;
        }
        else
        {
            stack.Remove(character);
        }
    }



    public void OnEvent(GameEventType eventType, object param = null)
    {
        if (!GameManager.Inst.Exploration.CheckArtifactID(55))
            return;
        switch (eventType)
        {
            case GameEventType.MonsterTakeDamage:
                if (stack[player] == maxStack
                    && condition != -1)
                {
                    GameManager.Inst.Exploration.player.HP += (int)param / 2;
                    stack[player] = 0;
                    condition = -1;
                }
                break;
            case GameEventType.MonsterDie:
                manager = (FightSceneManager)GameManager.Inst.manager;
                stack[player]++;
                if (stack[player] > maxStack)
                    stack[player] = maxStack;
                if (stack[player] == maxStack)
                    condition = GameManager.Inst.Exploration.TotalSkillUseCount;
                stackCount = stack[player];
                manager.SetArtifactValue(55, stack[player]);
                break;
        }
    }

    public void OnEvent(GameEventType eventType, object sender, object param = null)
    {
    }

}
