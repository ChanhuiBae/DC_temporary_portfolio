using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactListPage : PageUI
{
    private ArtifactPage artifactPage;
    private Button[] artifacts = new Button[20];
    private Image[] icons = new Image[20];
    private Entity_Artifact[] datas = new Entity_Artifact[20];

    private Button left;
    private Button right;
    private TextMeshProUGUI page;
    private int allpage;
    private int currentpage;

    private void Awake()
    {
        GameObject obj = GameObject.Find("ArtifactPage");
        if (obj == null || !obj.TryGetComponent<ArtifactPage>(out artifactPage))
        {
            Debug.Log("ArtifactListPage - Awake - ArtifactPage");
        }
        Transform t = transform.GetChild(0);
        for (int i = 0; i < 20; i++)
        {
            artifacts[i] = t.GetChild(i).GetComponent<Button>();
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
        allpage = GameManager.Inst.GetArtifactCount() / 20;
        if (GameManager.Inst.GetArtifactCount() % 20 > 0)
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
            artifacts[i].enabled = true;
            artifacts[i].onClick.RemoveAllListeners();
            if (start + i <= GameManager.Inst.GetArtifactCount())
            {
                GameManager.Inst.GetArtifactData(start + i, out datas[i]);
                icons[i].enabled = true;
                icons[i].sprite = Resources.Load<Sprite>("Artifact/" + datas[i].Icon);
            }
            else
            {
                icons[i].enabled = false;
                artifacts[i].enabled = false;
            }
        }
        artifactPage.SetPage(datas[0]);
        artifacts[0].onClick.AddListener(delegate { artifactPage.SetPage(datas[0]); });
        artifacts[1].onClick.AddListener(delegate { artifactPage.SetPage(datas[1]); });
        artifacts[2].onClick.AddListener(delegate { artifactPage.SetPage(datas[2]); });
        artifacts[3].onClick.AddListener(delegate { artifactPage.SetPage(datas[3]); });
        artifacts[4].onClick.AddListener(delegate { artifactPage.SetPage(datas[4]); });
        artifacts[5].onClick.AddListener(delegate { artifactPage.SetPage(datas[5]); });
        artifacts[6].onClick.AddListener(delegate { artifactPage.SetPage(datas[6]); });
        artifacts[7].onClick.AddListener(delegate { artifactPage.SetPage(datas[7]); });
        artifacts[8].onClick.AddListener(delegate { artifactPage.SetPage(datas[8]); });
        artifacts[9].onClick.AddListener(delegate { artifactPage.SetPage(datas[9]); });
        artifacts[10].onClick.AddListener(delegate { artifactPage.SetPage(datas[10]); });
        artifacts[11].onClick.AddListener(delegate { artifactPage.SetPage(datas[11]); });
        artifacts[12].onClick.AddListener(delegate { artifactPage.SetPage(datas[12]); });
        artifacts[13].onClick.AddListener(delegate { artifactPage.SetPage(datas[13]); });
        artifacts[14].onClick.AddListener(delegate { artifactPage.SetPage(datas[14]); });
        artifacts[15].onClick.AddListener(delegate { artifactPage.SetPage(datas[15]); });
        artifacts[16].onClick.AddListener(delegate { artifactPage.SetPage(datas[16]); });
        artifacts[17].onClick.AddListener(delegate { artifactPage.SetPage(datas[17]); });
        artifacts[18].onClick.AddListener(delegate { artifactPage.SetPage(datas[18]); });
        artifacts[19].onClick.AddListener(delegate { artifactPage.SetPage(datas[19]); });


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