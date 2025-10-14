using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartnerPage : PageUI
{
    private Image partner;
    private TextMeshProUGUI partnerName;
    private Image partnerSkill;
    private TextMeshProUGUI skillName;
    private Vector3[] skillBalls = new Vector3[5];
    private Image[] skill_icons = new Image[5];
    private EventTrigger[] triggers = new EventTrigger[5];
    private EventTrigger.Entry[] enters = new EventTrigger.Entry[5];
    private EventTrigger.Entry exit;
    private Entity_Skill[] skills = new Entity_Skill[5];
    private BookPopup popup;
    private void Awake()
    {
        Transform t = transform.Find("Character");
        if (t == null || !t.TryGetComponent<Image>(out partner))
        {
            Debug.Log("PartnerPage - Awake - Image");
        }
        t = transform.Find("Name");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out partnerName))
        {
            Debug.Log("PartnerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("SkillIcon");
        if (t == null || !t.TryGetComponent<Image>(out partnerSkill))
        {
            Debug.Log("PartnerPage - Awake - Image");
        }
        t = transform.Find("SkillName");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out skillName))
        {
            Debug.Log("PartnerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("SkillBalls");
        if(t != null)
        {
            for(int i = 0; i < t.childCount; i++)
            {
                skillBalls[i] = t.GetChild(i).transform.position;
            }
        }

        exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener(delegate { ClosePopup(); });

        t = transform.Find("Skills");
        if (t != null)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                skill_icons[i] = t.GetChild(i).GetComponent<Image>();
                triggers[i] = t.GetChild(i).GetComponent<EventTrigger>();
                enters[i] = new EventTrigger.Entry();
                enters[i].eventID = EventTriggerType.PointerEnter;
            }
            enters[0].callback.AddListener(delegate { ShowPopup(0); });
            enters[1].callback.AddListener(delegate { ShowPopup(1); });
            enters[2].callback.AddListener(delegate { ShowPopup(2); });
            enters[3].callback.AddListener(delegate { ShowPopup(3); });
            enters[4].callback.AddListener(delegate { ShowPopup(4); });
            for (int i = 0; i < t.childCount; i++)
            {
                triggers[i].triggers.Add(enters[i]);
                triggers[i].triggers.Add(exit);
            }
        }



        GameObject obj = GameObject.Find("BookPopup");
        if(obj == null || !obj.TryGetComponent<BookPopup>(out popup))
        {
            Debug.Log("PartnerPage - Awake - BookPopup");
        }
    }

    public void SetPage()
    {
        // partner.sprite = 
        // partnerName.text = 
        // partnerSkill.sprite = 
        // skillName.text = 
        for(int i = 0; i < 5; i++)
        {
            GameManager.Inst.GetSkillData(GameManager.Inst.Exploration.skills[i+1],out skills[i]);
        }
        for (int i = 0; i < 5; i++)
        {
            skill_icons[i].sprite = Resources.Load<Sprite>("Skill/" + skills[i].Icon);
        }
    }

    private void ShowPopup(int skill)
    {
        popup.gameObject.SetActive(true);
        popup.SetSkillPopup(skills[skill]);
    }

    private void ClosePopup()
    {
        popup.gameObject.SetActive(false);
    }
}
