using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillListPage : PageUI
{
    private SkillPage skillPage;
    private Button[] skills = new Button[20];
    private Image[] icons = new Image[20];
    private Entity_Skill[] datas = new Entity_Skill[20];

    private Button left;
    private Button right;
    private TextMeshProUGUI page;
    private int allpage;
    private int currentpage;

    private void Awake()
    {
        GameObject obj = GameObject.Find("SkillPage");
        if(obj == null || !obj.TryGetComponent<SkillPage>(out skillPage))
        {
            Debug.Log("SkillListPage - Awake - SkillPage");
        }
        Transform t = transform.GetChild(0);
        for (int i = 0; i <20; i++)
        {
            skills[i] = t.GetChild(i).GetComponent<Button>();
            icons[i] = t.GetChild(i).GetChild(0).GetComponent<Image>();
        }
        t = transform.GetChild(1);
        if(!t.TryGetComponent<TextMeshProUGUI>(out page))
        {
            Debug.Log("SkillListPage - Awake - TextMeshProUGUI");
        }
        t = transform.GetChild(2);
        if(!t.TryGetComponent<Button>(out right))
        {
            Debug.Log("SkillListPage - Awake - Button");
        }
        t = transform.GetChild(3);
        if (!t.TryGetComponent<Button>(out left))
        {
            Debug.Log("SkillListPage - Awake - Button");
        }
        currentpage = 1;
        left.onClick.AddListener(ClickLeft);
        right.onClick.AddListener(ClickRight);
    }

    private void Start()
    {
        allpage = GameManager.Inst.GetSkillCount() / 20;
        if (GameManager.Inst.GetSkillCount() % 20 > 0)
            allpage++;
    }

    public void SetPage(int count)
    {
        if(count <= allpage && count > 0)
        {
            currentpage = count;
            page.text = currentpage.ToString();
        }
        int end = currentpage * 20;
        int start = end - 19;
        for (int i = 0; i < 20; i++)
        {
            skills[i].enabled = true;
            skills[i].onClick.RemoveAllListeners();
            if (start + i <= GameManager.Inst.GetSkillCount())
            {
                GameManager.Inst.GetSkillData(start + i, out datas[i]);
                icons[i].enabled = true;
                if(start + i < 6)
                    icons[i].sprite = Resources.Load<Sprite>("Bubble/" + datas[i].Bubble);
                else
                    icons[i].sprite = Resources.Load<Sprite>("Skill/" + datas[i].Icon);
            }
            else
            {
                icons[i].enabled = false;
                skills[i].enabled = false;
            }
        }
        skillPage.SetPage(datas[0]);
        skills[0].onClick.AddListener(delegate { skillPage.SetPage(datas[0]); });
        skills[1].onClick.AddListener(delegate { skillPage.SetPage(datas[1]); });
        skills[2].onClick.AddListener(delegate { skillPage.SetPage(datas[2]); });
        skills[3].onClick.AddListener(delegate { skillPage.SetPage(datas[3]); });
        skills[4].onClick.AddListener(delegate { skillPage.SetPage(datas[4]); });
        skills[5].onClick.AddListener(delegate { skillPage.SetPage(datas[5]); });
        skills[6].onClick.AddListener(delegate { skillPage.SetPage(datas[6]); });
        skills[7].onClick.AddListener(delegate { skillPage.SetPage(datas[7]); });
        skills[8].onClick.AddListener(delegate { skillPage.SetPage(datas[8]); });
        skills[9].onClick.AddListener(delegate { skillPage.SetPage(datas[9]); });
        skills[10].onClick.AddListener(delegate { skillPage.SetPage(datas[10]); });
        skills[11].onClick.AddListener(delegate { skillPage.SetPage(datas[11]); });
        skills[12].onClick.AddListener(delegate { skillPage.SetPage(datas[12]); });
        skills[13].onClick.AddListener(delegate { skillPage.SetPage(datas[13]); });
        skills[14].onClick.AddListener(delegate { skillPage.SetPage(datas[14]); });
        skills[15].onClick.AddListener(delegate { skillPage.SetPage(datas[15]); });
        skills[16].onClick.AddListener(delegate { skillPage.SetPage(datas[16]); });
        skills[17].onClick.AddListener(delegate { skillPage.SetPage(datas[17]); });
        skills[18].onClick.AddListener(delegate { skillPage.SetPage(datas[18]); });
        skills[19].onClick.AddListener(delegate { skillPage.SetPage(datas[19]); });
    }

    private void ClickLeft()
    {
        if (currentpage != 1)
        {
            SetPage(--currentpage);
        }
    }

    private void ClickRight()
    {
        if (currentpage != allpage)
        {
            SetPage(++currentpage);
        }
    }
}
