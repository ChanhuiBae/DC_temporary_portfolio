using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopup : MonoBehaviour
{
    private ExplorationManager manager;
    private SkillChoice[] choices;

    private void Awake()
    {
        choices = new SkillChoice[3];
        for(int i = 0; i < 3; i++)
        {
            choices[i] = transform.GetChild(i).GetComponent<SkillChoice>();
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        gameObject.SetActive(false);
    }

    public void Close()
    {
        manager.StartNextVictoryLogic();
        gameObject.SetActive(false);
    }
}
