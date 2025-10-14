using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightSceneManager : MainManager
{
    private CharacterInfo playerInfo;
    private CharacterController partner;
    private GameObject target;
    private Fade fade;
    private ShowText ready;
    private ShowText fight;
    private ShowText victory;
    private ShowText fever;

    private Dictionary<int, SkillBallUI> skillBalls;
    private Dictionary<int, ArtifactFight> artifacts;
    private SkillBallManager ballManager;
    private SkillQueue queueManager;

    private Button potionBtn;
    private TextMeshProUGUI potionCount;
    private DamageTextManager damageText;

    private void Awake()
    {
        base.Awake();
        GameObject p = GameObject.Find("PlayerInfo");
        if (p == null || !p.TryGetComponent<CharacterInfo>(out playerInfo))
        {
            Debug.Log("FightSceneManager - Awake - CharacterInfo");
        }
        p = GameObject.Find("Partner");
        if (p == null || !p.TryGetComponent<CharacterController>(out partner))
        {
            Debug.Log("FightSceneManager - Awake - CharacterController");
        }
        target = GameObject.Find("Target");
        GameObject f = GameObject.Find("Fade");
        if(f == null || !f.TryGetComponent<Fade>(out fade))
        {
            Debug.Log("FightSceneManager - Awake - Fade");
        }
        GameObject text = GameObject.Find("Ready");
        if(text == null || !text.TryGetComponent<ShowText>(out ready))
        {
            Debug.Log("FightSceneManager - Awake - ShowText");
        }
        text = GameObject.Find("Fight");
        if (text == null || !text.TryGetComponent<ShowText>(out fight))
        {
            Debug.Log("FightSceneManager - Awake - ShowText");
        }
        text = GameObject.Find("Victory");
        if (text == null || !text.TryGetComponent<ShowText>(out victory))
        {
            Debug.Log("FightSceneManager - Awake - ShowText");
        }
        text = GameObject.Find("FeverText");
        if (text == null || !text.TryGetComponent<ShowText>(out fever))
        {
            Debug.Log("FightSceneManager - Awake - ShowText");
        }
        GameObject m = GameObject.Find("SkillQueue");
        if(m == null || !m.TryGetComponent<SkillQueue>(out queueManager))
        {
            Debug.Log("FightSceneManager - Awake - SkillQueue");
        }
        m = GameObject.Find("SkillBallManager");
        if(m == null || !m.TryGetComponent<SkillBallManager>(out ballManager))
        {
            Debug.Log("FightSceneManager - Awake - SkillBallManager");
        }
        skillBalls = new Dictionary<int, SkillBallUI>();
        GameObject active = GameObject.Find("Active");
        if(active != null)
        {
            for(int i = 0; i < active.transform.childCount; i++)
            {
                skillBalls.Add(i + 1, active.transform.GetChild(i).GetComponent<SkillBallUI>());
            }
        }
        artifacts = new Dictionary<int, ArtifactFight>();
        GameObject artifact = GameObject.Find("Artifacts");
        if(artifact != null)
        {
            for(int i = 0; i < artifact.transform.childCount; i++)
            {
                artifacts.Add(i + 1, artifact.transform.GetChild(i).GetComponent<ArtifactFight>());
            }
        }
        GameObject potion = GameObject.Find("PotionButton");
        if(potion == null || !potion.TryGetComponent<Button>(out potionBtn))
        {
            Debug.Log("FightSceneManager - Awake - Button");
        }
        else
        {
            potionBtn.onClick.AddListener(UsePotion);
        }
        potion = GameObject.Find("PotionCount");
        if(potion == null || !potion.TryGetComponent<TextMeshProUGUI>(out potionCount))
        {
            Debug.Log("FightSceneManager - Awake - TextMeshProUGUI");
        }
        else
        {
            potionCount.text = GameManager.Inst.Exploration.player.invenory.GetItemAmount(1).ToString(); // potion id = 1
        }
        GameObject obj = GameObject.Find("DamageTextManager");
        if(obj == null || !obj.TryGetComponent<DamageTextManager>(out damageText))
        {
            Debug.Log("FightSceneManager - Awake - DamageTextManager");
        }

    }

    private void Start()
    {
        playerInfo.SetHP(GameManager.Inst.Exploration.player.HP / GameManager.Inst.Exploration.player.MaxHP);
        for (int i = 1; i <= skillBalls.Count; i++)
        {
            int id;
            if (GameManager.Inst.Exploration.skills.TryGetValue(i, out id))
            {
                skillBalls[i].SetSkillBall(id);
            }
            else
            {
                skillBalls[i].SetSkillBall(0);
            }
        }
        for(int i = 1; i <= artifacts.Count; i++)
        {
            Artifact artifact;
            if(GameManager.Inst.Exploration.artifacts.TryGetValue(i, out artifact))
            {
                artifacts[i].SetArtifact(artifact.GetID(true));
            }
        }

        fade.FadeOut(1);
        StartCoroutine(PlayStartEffect());
    }

    private IEnumerator PlayStartEffect()
    {
        StartCoroutine(StartMove(1));
        StartCoroutine(ready.ShowAndDisappear());
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        StartCoroutine(fight.ShowAndDisappear());
        GameManager.Inst.Fight = true;
        GameManager.Inst.eventManager.PostNotification(GameEventType.FightStart);

    }

    private IEnumerator StartMove(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        target.SetActive(false);
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Run);
        partner.PlayAnimation(CharacterStateDC.Run);
        GameManager.Inst.player.transform.LeanMoveX(-9, 1f);
        partner.transform.LeanMoveX(-12, 1f);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Ready);
        partner.PlayAnimation(CharacterStateDC.Ready);
        target.SetActive(true);
    }

    public void Fever()
    {
        StartCoroutine(fever.ShowAndDisappearSequentially());
    }

    public void Victory()
    {
        GameManager.Inst.player.Control = false;
        GameManager.Inst.Fight = false;
        ballManager.Stop();
        queueManager.Stop();
        StartCoroutine(victory.ShowSequentially());
        
        StartCoroutine(GoExploration());
    }

    private IEnumerator GoExploration()
    {
        yield return YieldInstructionCache.WaitForSeconds(2f);
        fade.FadeIn(1);
        StartCoroutine(WaitLoadScene(SceneName.ExplorationScene, 1));
    }

    public void Lose()
    {
        GameManager.Inst.player.Control = false;
        GameManager.Inst.Fight = false;
        ballManager.Stop();
        queueManager.Stop();
    }

    private void UsePotion()
    {
        if(GameManager.Inst.itemManager.UseItem(1)) //potion id = 1
        {
            potionCount.text = GameManager.Inst.Exploration.player.invenory.GetItemAmount(1).ToString();
        }
    }
    public override void SetHP()
    {
        playerInfo.SetHP(GameManager.Inst.Exploration.player.HP / GameManager.Inst.Exploration.player.MaxHP);
    }

    public void ShowDamage(Vector3 position, int damage, Color color, bool critical)
    {
        damageText.SetDamage(position, damage, color, critical);
    }

    public void SetArtifactValue(int id, int value)
    {
        for(int i = 1; i < 6; i++)
        {
            if(artifacts.ContainsKey(i) && artifacts[i].ID == id)
            {
                artifacts[i].SetValue(value);
                break;
            }
        }
    }

    public int GetArtifactValue(int id)
    {
        for (int i = 1; i < 6; i++)
        {
            if (artifacts.ContainsKey(i) && artifacts[i].ID == id)
            {
                return System.Int32.Parse(artifacts[i].GetValue());
            }
        }
        return -1;
    }
}
