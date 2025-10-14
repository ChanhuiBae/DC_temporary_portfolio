using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillPage : PageUI
{
    private Image icon;
    private TextMeshProUGUI name;
    private TextMeshProUGUI description;
    private Image attackArea;
    private GameObject area;

    private void Awake()
    {
        Transform t = transform.Find("Icon");
        if (t == null || !t.TryGetComponent<Image>(out icon))
        {
            Debug.Log("SkillPage - Awake - Image");
        }
        t = transform.Find("Name");
        if(t == null || !t.TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("SkillPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Description");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out description))
        {
            Debug.Log("SkillPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("AttackArea");
        if (t == null || !t.TryGetComponent<Image>(out attackArea))
        {
            Debug.Log("SkillPage - Awake - Image");
        }
        area = transform.Find("Area").gameObject;
    }

    public void SetPage(Entity_Skill skill)
    {
        if(skill.ID < 6) 
            icon.sprite = Resources.Load<Sprite>("Bubble/" + skill.Bubble);
        else
            icon.sprite = Resources.Load<Sprite>("Skill/" + skill.Icon);
        name.text = skill.Name;
        description.text = skill.Exploration;
        if(skill.AttackArea == "None")
        {
            area.SetActive(false);
            attackArea.enabled = false;
        }
        else
        {
            area.SetActive(true);
            attackArea.enabled = true;
            attackArea.sprite = Resources.Load<Sprite>("AttackArea/" + skill.AttackArea);
        }
    }
}
