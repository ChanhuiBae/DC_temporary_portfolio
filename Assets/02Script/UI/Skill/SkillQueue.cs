using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillQueue : MonoBehaviour
{
    private SkillManager manager;
    private Vector3[] positions = new Vector3[10];
    private bool fever;
    public bool Fever
    {
        set => fever = value;
        get => fever;
    }
    public Vector3 GetUp(Vector3 pos)
    {
        int index = Array.IndexOf(positions, pos);
        if(index > 0)
        {
            return positions[index - 1];
        }
        return pos;
    }
    private Dictionary<SkillItem, bool> skillItems = new Dictionary<SkillItem, bool>();
    private Queue<SkillItem> ready = new Queue<SkillItem>();

    private void Awake()
    {
        GameObject skillM = GameObject.Find("SkillManager");
        if(skillM == null || !skillM.TryGetComponent<SkillManager>(out manager))
        {
            Debug.Log("SkillQueue - Awake - SkillManager");
        }
        for(int i = 0; i < 10; i++)
        {
            positions[i] = transform.GetChild(0).GetChild(i).transform.position;
            skillItems.Add(transform.GetChild(0).GetChild(i).GetComponent<SkillItem>(),false);
        }
    }

    public bool AddReadySkill(int id, int skillballCount)
    {
        if (ready.Count == 10)
            return false;
        foreach(var i in skillItems)
        {
            if (!ready.Contains(i.Key))
            {
                i.Key.Init(id, positions[ready.Count], skillballCount);
                ready.Enqueue(i.Key);
                GameManager.Inst.Exploration.SkillQueueCount = ready.Count;
                break;
            }
        }
        return true;
    }

    public void SetReady(SkillItem item,bool value)
    {
        skillItems[item] = value;    
    }

    public void Dequeue()
    {
        ready.Dequeue();
        GameManager.Inst.Exploration.UsingSkill = true;
        GameManager.Inst.Exploration.SkillQueueCount = ready.Count;
        SkillItem[] arr = ready.ToArray();
        for(int i = 0; i < arr.Count(); i++)
        {
            arr[i].MoveUp(positions[i]);
        }

    }

    private void Update()
    {
        if(ready.Count > 0)
        {
            if (skillItems[ready.Peek()] 
                && manager.UseSkill(ready.Peek().ID, ready.Peek().Skillball, ready.Peek().Fever)) 
            {
                skillItems[ready.Peek()] = false;
                ready.Peek().StartFlash();
            }
        }
    }
    public void Stop()
    {
        enabled = false;
    }
}
