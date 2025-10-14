using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBallUI : MonoBehaviour
{
    private Image bubble;
    private Image icon;

    private void Awake()
    {
        if (!TryGetComponent<Image>(out bubble))
        {
            Debug.Log("ArtifactUI - Awake - Image");
        }
        if (!transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("ArtifactUI - Awake - Image");
        }
    }

    public void SetSkillBall(int id)
    {
        if (id == 0)
        {
            icon.enabled = false;
        }
        else
        {
            icon.enabled = true;
            Entity_Skill skill;
            GameManager.Inst.GetSkillData(id, out skill);
            bubble.sprite = Resources.Load<Sprite>("Bubble/" + skill.Bubble);
            if(id > 5)
            {
                icon.enabled = true;
                icon.sprite = Resources.Load<Sprite>("Skill/" + skill.Icon);
            }
            else
                icon.enabled= false;
        }
    }
}
