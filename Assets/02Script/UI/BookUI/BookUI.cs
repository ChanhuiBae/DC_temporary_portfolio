using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    private Button journalBtn;
    private Button monsterBtn;
    private Button artifactBtn;
    private Button skillBtn;
    private Button close;

    private Button current;

    private PlayerPage playerPage;
    private PartnerPage partnerPage;
    private MonsterListPage mlistPage;
    private MonsterPage monsterPage;
    private ArtifactListPage alistPage;
    private ArtifactPage artifactPage;
    private SkillListPage slistPage;
    private SkillPage skillPage;

    private PageUI left;
    private PageUI right;

    private void Awake()
    {
        GameObject btn = GameObject.Find("JournalBtn");
        if (btn == null || !btn.TryGetComponent<Button>(out journalBtn))
        {
            Debug.Log("BookUI - Awake - Button");
        }
        btn = GameObject.Find("MonsterBtn");
        if (btn == null || !btn.TryGetComponent<Button>(out monsterBtn))
        {
            Debug.Log("BookUI - Awake - Button");
        }
        btn = GameObject.Find("ArtifactBtn");
        if (btn == null || !btn.TryGetComponent<Button>(out artifactBtn))
        {
            Debug.Log("BookUI - Awake - Button");
        }
        btn = GameObject.Find("SkillBtn");
        if (btn == null || !btn.TryGetComponent<Button>(out skillBtn))
        {
            Debug.Log("BookUI - Awake - Button");
        }
        btn = GameObject.Find("CloseBtn");
        if (btn == null || !btn.TryGetComponent<Button>(out close))
        {
            Debug.Log("BookUI - Awake - Button");
        }
        close.onClick.AddListener(Close);

        GameObject page = GameObject.Find("PlayerPage");
        if(page == null || !page.TryGetComponent<PlayerPage>(out playerPage))
        {
            Debug.Log("BookUI - Awake - PlayerPage");
        }
        page = GameObject.Find("PartnerPage");
        if (page == null || !page.TryGetComponent<PartnerPage>(out partnerPage))
        {
            Debug.Log("BookUI - Awake - PartnerPage");
        }
        page = GameObject.Find("MonsterListPage");
        if (page == null || !page.TryGetComponent<MonsterListPage>(out mlistPage))
        {
            Debug.Log("BookUI - Awake - MonsterListPage");
        }
        page = GameObject.Find("MonsterPage");
        if (page == null || !page.TryGetComponent<MonsterPage>(out monsterPage))
        {
            Debug.Log("BookUI - Awake - MonsterPage");
        }
        page = GameObject.Find("ArtifactListPage");
        if (page == null || !page.TryGetComponent<ArtifactListPage>(out alistPage))
        {
            Debug.Log("BookUI - Awake - ArtifactListPage");
        }
        page = GameObject.Find("ArtifactPage");
        if (page == null || !page.TryGetComponent<ArtifactPage>(out artifactPage))
        {
            Debug.Log("BookUI - Awake - ArtifactPage");
        }
        page = GameObject.Find("SkillListPage");
        if (page == null || !page.TryGetComponent<SkillListPage>(out slistPage))
        {
            Debug.Log("BookUI - Awake - SkillListPage");
        }
        page = GameObject.Find("SkillPage");
        if (page == null || !page.TryGetComponent<SkillPage>(out skillPage))
        {
            Debug.Log("BookUI - Awake - SkillPage");
        }

        journalBtn.onClick.AddListener(() => { StartCoroutine(OpenJournal()); });
        monsterBtn.onClick.AddListener(() => { StartCoroutine(OpenMonster()); });
        artifactBtn.onClick.AddListener(() => { StartCoroutine(OpenArtifact()); });
        skillBtn.onClick.AddListener(() => { StartCoroutine(OpenSkill()); });
    }

    private void Start()
    {
        current = journalBtn;
        current.transform.LeanMoveLocalX(-500f, 0);
        current.enabled = false;
        left = playerPage;
        right = partnerPage;
        mlistPage.transform.LeanRotateY(90, 0);
        monsterPage.transform.LeanRotateY(90, 0);
        alistPage.transform.LeanRotateY(90, 0);
        artifactPage.transform.LeanRotateY(90, 0);
        slistPage.transform.LeanRotateY(90, 0);
        skillPage.transform.LeanRotateY(90, 0); 

        gameObject.SetActive(false);
    }

    public void ShowFirst()
    {
        playerPage.SetPage();
        partnerPage.SetPage();
    }

    private void Close()
    {
        ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
        manager.SetControlTrue();
        gameObject.SetActive(false);
    }

    private IEnumerator OpenJournal()
    {
        SetButtonEnable(false);
        current.transform.LeanMoveLocalX(-480f, 0.2f);
        journalBtn.transform.LeanMoveLocalX(-500f, 0.2f);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        current = journalBtn;

        playerPage.SetPage();
        partnerPage.SetPage();

        partnerPage.transform.SetAsFirstSibling();
        partnerPage.Show();
        right.Close();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.transform.SetAsFirstSibling();
        playerPage.Open();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.Close();
        left = playerPage;
        right = partnerPage;

        SetButtonEnable(true);
    }

    private IEnumerator OpenMonster()
    {
        SetButtonEnable(false);
        current.transform.LeanMoveLocalX(-480f, 0.2f);
        monsterBtn.transform.LeanMoveLocalX(-500f, 0.2f);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        current = monsterBtn;

        mlistPage.SetPage(1);

        monsterPage.transform.SetAsFirstSibling();
        monsterPage.Show();
        right.Close();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.transform.SetAsFirstSibling();
        mlistPage.Open();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.Close();
        left = mlistPage;
        right = monsterPage;

        SetButtonEnable(true);
    }
    private IEnumerator OpenArtifact()
    {
        SetButtonEnable(false);
        current.transform.LeanMoveLocalX(-480f, 0.2f);
        artifactBtn.transform.LeanMoveLocalX(-500f, 0.2f);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        current = artifactBtn;

        alistPage.SetPage(1);

        artifactPage.transform.SetAsFirstSibling();
        artifactPage.Show();
        right.Close();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.transform.SetAsFirstSibling();
        alistPage.Open();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.Close();
        left = alistPage;
        right = artifactPage;

        SetButtonEnable(true);
    }
    private IEnumerator OpenSkill()
    {
        SetButtonEnable(false);
        current.transform.LeanMoveLocalX(-480f, 0.2f);
        skillBtn.transform.LeanMoveLocalX(-500f, 0.2f);
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        current = skillBtn;

        slistPage.SetPage(1);

        skillPage.transform.SetAsFirstSibling();
        skillPage.Show();
        right.Close();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.transform.SetAsFirstSibling();
        slistPage.Open();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        left.Close();
        left = slistPage;
        right = skillPage;

        SetButtonEnable(true);
    }

    private void SetButtonEnable(bool value)
    {
        journalBtn.enabled = value;
        monsterBtn.enabled = value;
        artifactBtn.enabled = value;
        skillBtn.enabled = value;
        current.enabled = false;
    }
}
