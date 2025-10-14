using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterListPage : PageUI
{
    private MonsterPage monsterPage;
    private Button[] monsters = new Button[20];
    private Image[] icons = new Image[20];
    private Entity_Monster[] datas = new Entity_Monster[20];

    private Button left;
    private Button right;
    private TextMeshProUGUI page;
    private int allpage;
    private int currentpage;

    private void Awake()
    {
        GameObject obj = GameObject.Find("MonsterPage");
        if (obj == null || !obj.TryGetComponent<MonsterPage>(out monsterPage))
        {
            Debug.Log("ArtifactListPage - Awake - ArtifactPage");
        }
        Transform t = transform.GetChild(0);
        for (int i = 0; i < 20; i++)
        {
            monsters[i] = t.GetChild(i).GetComponent<Button>();
            icons[i] = t.GetChild(i).GetChild(0).GetComponent<Image>();
        }
        t = transform.GetChild(1);
        if (!t.TryGetComponent<TextMeshProUGUI>(out page))
        {
            Debug.Log("SkillListPage - Awake - TextMeshProUGUI");
        }
        t = transform.GetChild(2);
        if (!t.TryGetComponent<Button>(out right))
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
        allpage = GameManager.Inst.GetMonsterCount() / 20;
        if (GameManager.Inst.GetMonsterCount() % 20 > 0)
            allpage++;
    }

    public void SetPage(int count)
    {
        if (count <= allpage && count > 0)
        {
            currentpage = count;
            page.text = currentpage.ToString();
        }
        int end = currentpage * 20;
        int start = end - 19;
        for (int i = 0; i < 20; i++)
        {
            monsters[i].enabled = true;
            monsters[i].onClick.RemoveAllListeners();
            if (start + i <= GameManager.Inst.GetMonsterCount())
            {
                GameManager.Inst.GetMonsterSearchByIndex(start + i - 1, out datas[i]);
                icons[i].enabled = true;
                icons[i].sprite = Resources.Load<Sprite>("Profile/" + datas[i].Path);
            }
            else
            {
                icons[i].enabled = false;
                monsters[i].enabled = false;
            }
        }
        monsterPage.SetPage(datas[0]);
        monsters[0].onClick.AddListener(delegate { monsterPage.SetPage(datas[0]); });
        monsters[1].onClick.AddListener(delegate { monsterPage.SetPage(datas[1]); });
        monsters[2].onClick.AddListener(delegate { monsterPage.SetPage(datas[2]); });
        monsters[3].onClick.AddListener(delegate { monsterPage.SetPage(datas[3]); });
        monsters[4].onClick.AddListener(delegate { monsterPage.SetPage(datas[4]); });
        monsters[5].onClick.AddListener(delegate { monsterPage.SetPage(datas[5]); });
        monsters[6].onClick.AddListener(delegate { monsterPage.SetPage(datas[6]); });
        monsters[7].onClick.AddListener(delegate { monsterPage.SetPage(datas[7]); });
        monsters[8].onClick.AddListener(delegate { monsterPage.SetPage(datas[8]); });
        monsters[9].onClick.AddListener(delegate { monsterPage.SetPage(datas[9]); });
        monsters[10].onClick.AddListener(delegate { monsterPage.SetPage(datas[10]); });
        monsters[11].onClick.AddListener(delegate { monsterPage.SetPage(datas[11]); });
        monsters[12].onClick.AddListener(delegate { monsterPage.SetPage(datas[12]); });
        monsters[13].onClick.AddListener(delegate { monsterPage.SetPage(datas[13]); });
        monsters[14].onClick.AddListener(delegate { monsterPage.SetPage(datas[14]); });
        monsters[15].onClick.AddListener(delegate { monsterPage.SetPage(datas[15]); });
        monsters[16].onClick.AddListener(delegate { monsterPage.SetPage(datas[16]); });
        monsters[17].onClick.AddListener(delegate { monsterPage.SetPage(datas[17]); });
        monsters[18].onClick.AddListener(delegate { monsterPage.SetPage(datas[18]); });
        monsters[19].onClick.AddListener(delegate { monsterPage.SetPage(datas[19]); });
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
