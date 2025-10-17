using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ExplorationManager : MainManager, UIManager
{
    private PiMenu pimenu;
    private CameraController camController;
    private MapManager mapManager;
    private Image mapFade;
    private Fade fade;
    private Canvas canvas;

    private Button messagePopup;
    private TextMeshProUGUI message;
    private TextMeshProUGUI text0;
    private Button textButton0;
    private TextMeshProUGUI text1;
    private Button textButton1;
    private TextMeshProUGUI text2;
    private Button textButton2;
    private TextMeshProUGUI text3;
    private Button textButton3;

    private ChestPopup chest;
    private BubbleManager bubbleManager;
    private InventoryUI inventory;
    private ShopPopup shop;
    private BookUI book;

    private FullPopup full;
    private ArtifactPopup artifactPopup;

    private Image lantern;
    private CharacterUI playerUI;

    private Dictionary<int, ArtifactUI> artifacts;
    private Bubble clickArtifact;

    private LootPopup lootpopup;

    private ExplanationPopup explanation;
    private Image explanPopup;
    private LevelUpPopup levelup;
    private Image exp;
    private TextMeshProUGUI level;
    private TextMeshProUGUI pearl;
    private TextMeshProUGUI pearlPlus;

    private EventTrigger trigger;
    private EventTrigger.Entry click;

    private VictoryLogic victory = new VictoryLogic();
    private NotifyLogic levelupEvent = new NotifyLogic();
    private NotifyLogic getlootEvent = new NotifyLogic();
    private NotifyLogic merchant = new NotifyLogic();

    private void Awake()
    {
        base.Awake();
        levelupEvent.notifyEvent += LevelUp;
        getlootEvent.notifyEvent += GetLoot;
        merchant.notifyEvent += StartMerchantEvent;

        GameObject obj = GameObject.Find("CanvasOrder3");
        if (obj == null || !obj.TryGetComponent<Canvas>(out canvas))
        {
            Debug.Log("ExplorationManager - Awake - Canvas");
        }
        else
            canvas.sortingOrder = 3;
        
        obj = GameObject.Find("LootPopup");
        if (obj == null || !obj.TryGetComponent<LootPopup>(out lootpopup))
        {
            Debug.Log("ExplorationManger - Awake - LootPopup");
        }
        obj = GameObject.Find("PiMenu");
        if (obj == null || !obj.TryGetComponent<PiMenu>(out pimenu))
        {
            Debug.Log("ExplorationManager - Awake - PiMenu");
        }
        obj = GameObject.Find("CameraController");
        if (obj == null || !obj.transform.TryGetComponent<CameraController>(out camController))
        {
            Debug.Log("ExplorationManager - Awake - CameraController");
        }
        obj = GameObject.Find("MapManager");
        if (obj == null || !obj.transform.TryGetComponent<MapManager>(out mapManager))
        {
            Debug.Log("ExplorationManager - Awake - MapManager");
        }
        obj = GameObject.Find("MapFade");
        if (obj == null || !obj.TryGetComponent<Image>(out mapFade))
        {
            Debug.Log("ExplorationManager - Awake - Image");
        }
        obj = GameObject.Find("Fade");
        if (obj == null || !obj.TryGetComponent<Fade>(out fade))
        {
            Debug.Log("ExplorationManager - Awake - Fade");
        }

        obj = GameObject.Find("EXP");
        if(obj == null || !obj.TryGetComponent<Image>(out exp))
        {
            Debug.Log("ExplorationManager - Awake - Image");
        }
        obj = GameObject.Find("Level");
        if (obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out level))
        {
            Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
        }
        obj = GameObject.Find("Pearl");
        if(obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out pearl))
        {
            Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
        }
        if (pearl == null || !pearl.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out pearlPlus))
        {
            Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
        }
        obj = GameObject.Find("MessagePopup");
        if (obj == null || !obj.TryGetComponent<Button>(out messagePopup))
        {
            Debug.Log("ExplorationManager - Awake - Button");
        }
        else
        {
            if (!messagePopup.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out message))
            {
                Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
            }
            if (!messagePopup.transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out text0))
            {
                Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
            }
            if (!messagePopup.transform.GetChild(1).TryGetComponent<Button>(out textButton0))
            {
                Debug.Log("ExplorationManager - Awake - Button");
            }
            if (!messagePopup.transform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out text1))
            {
                Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
            }
            if (!messagePopup.transform.GetChild(2).TryGetComponent<Button>(out textButton1))
            {
                Debug.Log("ExplorationManager - Awake - Button");
            }
            if (!messagePopup.transform.GetChild(3).TryGetComponent<TextMeshProUGUI>(out text2))
            {
                Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
            }
            if (!messagePopup.transform.GetChild(3).TryGetComponent<Button>(out textButton2))
            {
                Debug.Log("ExplorationManager - Awake - Button");
            }
            if (!messagePopup.transform.GetChild(4).TryGetComponent<TextMeshProUGUI>(out text3))
            {
                Debug.Log("ExplorationManager - Awake - TextMeshProUGUI");
            }
            if (!messagePopup.transform.GetChild(4).TryGetComponent<Button>(out textButton3))
            {
                Debug.Log("ExplorationManager - Awake - Button");
            }
        }

        obj = GameObject.Find("ChestPopup");
        if (obj == null || !obj.TryGetComponent<ChestPopup>(out chest))
        {
            Debug.Log("ExplorationManager - Awake - ChestPopup");
        }
        obj = GameObject.Find("BubbleManager");
        if (obj == null || !obj.TryGetComponent<BubbleManager>(out bubbleManager))
        {
            Debug.Log("ExplorationManager - Awake - BubbleManager");
        }
        obj = GameObject.Find("Inventory");
        if (obj == null || !obj.TryGetComponent<InventoryUI>(out inventory))
        {
            Debug.Log("ExplorationManager - Awake - InventoryUI");
        }
        obj = GameObject.Find("Shop");
        if(obj == null || !obj.TryGetComponent<ShopPopup>(out shop))
        {
            Debug.Log("ExplorationManager - Awake - ShopPopup");
        }
        obj = GameObject.Find("BookUI");
        if(obj == null || !obj.TryGetComponent<BookUI>(out book))
        {
            Debug.Log("ExplorationManager - Awake - BookUI");
        }

        obj = GameObject.Find("FullPopup");
        if (obj == null || !obj.TryGetComponent<FullPopup>(out full))
        {
            Debug.Log("ExplorationManager - Awake - FullPopup");
        }
        obj = GameObject.Find("ArtifactPopup");
        if (obj == null || !obj.TryGetComponent<ArtifactPopup>(out artifactPopup))
        {
            Debug.Log("ExplorationManager - Awake - ArtifactPopup");
        }

        obj = GameObject.Find("Lantern");
        if (obj == null || !obj.transform.TryGetComponent<Image>(out lantern))
        {
            Debug.Log("ExplorationManager - Awake - Image");
        }
        obj = GameObject.Find("CharacterUI");
        if (obj != null)
        {
            if (!obj.transform.TryGetComponent<CharacterUI>(out playerUI))
            {
                Debug.Log("ExplorationManager - Awake - CharacterUI");
            }
        }

        artifacts = new Dictionary<int, ArtifactUI>();
        obj = GameObject.Find("Artifacts");
        if (obj != null)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                artifacts.Add(i+1, obj.transform.GetChild(i).GetComponent<ArtifactUI>());
            }
        }

        obj = GameObject.Find("ExplanationPopup");
        if(obj == null || !obj.TryGetComponent<ExplanationPopup>(out explanation))
        {
            Debug.Log("ExplorationManager - Awake - ExplanationPopup");
        }
        if(obj == null || !obj.TryGetComponent<Image>(out explanPopup))
        {
            Debug.Log("ExplaorationManager - Awake - Image");
        }

        obj = GameObject.Find("LevelUpPopup");
        if (obj == null || !obj.TryGetComponent<LevelUpPopup>(out levelup))
        {
            Debug.Log("ExplorationManager - Awake - LevelUpPopup");
        }

        obj = GameObject.Find("CameraController");
        if (obj == null || !obj.TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("ExplorationManager - Awake -  EventTrigger");
        }
        else
        {
            click = new EventTrigger.Entry();
            click.eventID = EventTriggerType.PointerClick;
            click.callback.AddListener((data) => { OnClick((PointerEventData)data); });
            trigger.triggers.Add(click);
        }

        ////////////// Artifact Test /////////////////
        if (!GameManager.Inst.Exploration.CheckArtifactID(54))
        {
            Artifact artifact = new Artifact(54);
            GameManager.Inst.Exploration.artifacts.Add(1, artifact);
        }
    }

    private void Start()
    {
        // RESET
        pearlPlus.enabled = false;
        pearl.text = GameManager.Inst.Exploration.player.Pearl.ToString();
        text2.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);
        playerUI.DrawHP();
        for (int i = 1; i < artifacts.Count+1; i++)
        {
            Artifact artifact;
            if (GameManager.Inst.Exploration.artifacts.TryGetValue(i, out artifact))
            {
                artifacts[i].SetBubble(artifact.Data.Grade);
                artifacts[i].SetArtifact(artifact.GetID(true));
                GameManager.Inst.Exploration.SetArtifact(i, artifact.GetID(true));
            }
            else
            {
                artifacts[i].SetBubble(0);
                artifacts[i].SetArtifact(0);
            }
        }
        camController.enabled = true;
        pimenu.gameObject.SetActive(false);
        messagePopup.gameObject.SetActive(false);

        // Victory Check
        if (GameManager.Inst.Exploration.GetSquad() != null && GameManager.Inst.Exploration.squadClear)
        {
            GameManager.Inst.Exploration.Satiety -= 10;
            StartCoroutine(WaitAddNotifyLogic((int)VictoryEvent.GetLoot, getlootEvent));
            switch (GameManager.Inst.Exploration.GetSquad().Type)
            {
                case 0:
                    StartCoroutine(SetEXP(80));
                    SetPearl(20);
                    break;
                case 1:
                    StartCoroutine(SetEXP(110));
                    SetPearl(30);
                    break;
                case 2:
                    StartCoroutine(SetEXP(150));
                    SetPearl(40);
                    break;
            }
            victory.StartVictoryLogic();
        }
        Entity_Squad squad = GameManager.Inst.Exploration.GetSquad();
        if (squad != null && squad.Type == 2 && GameManager.Inst.Exploration.squadClear)
        {
            StartCoroutine(WaitAddNotifyLogic((int)VictoryEvent.Merchant, merchant));
        }

        GameManager.Inst.Exploration.InitSkillUseCount();

        ///////////////// Skill Test////////////////////
        GameManager.Inst.Exploration.SetSkill(1, 13);
        GameManager.Inst.Exploration.SetSkill(2, 11);
        GameManager.Inst.Exploration.SetSkill(5, 10);
        GameManager.Inst.Exploration.SetSkill(3, 8);
    }

    private void OnClick(PointerEventData data)
    {
        if (GameManager.Inst.player.Control && data.button == PointerEventData.InputButton.Left)
            GameManager.Inst.player.Move(GameManager.Inst.Exploration.map.Mapdata.GetDirection(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f))));
        else if (data.button == PointerEventData.InputButton.Right)
        {
            if(GameManager.Inst.Exploration.map.Access && GameManager.Inst.player.State == CharacterStateDC.Idle)
            {
                if (GameManager.Inst.player.Control)
                {
                    GameManager.Inst.player.Control = false;
                    pimenu.gameObject.SetActive(true);
                    pimenu.Enable();
                }
                else if(pimenu.gameObject.activeSelf)
                {
                    GameManager.Inst.player.Control = true;
                    pimenu.gameObject.SetActive(false);
                }
            }
        }
    }

    public void FadeIn(float time)
    {
        mapFade.enabled = true;
        Color fromColor = new Color(0, 0, 0, 0);
        Color toColor = new Color(0, 0, 0, 1);
        LeanTween.value(mapFade.gameObject, setFadeCallback, fromColor, toColor, time);
        StartCoroutine(DisableMapFade(time));
    }

    public void FadeOut(float time)
    {
        mapFade.enabled = true;
        Color fromColor = new Color(0, 0, 0, 1);
        Color toColor = new Color(0, 0, 0, 0);
        LeanTween.value(mapFade.gameObject, setFadeCallback, fromColor, toColor, time);
        StartCoroutine(DisableMapFade(time));
    }

    private void setFadeCallback(Color c)
    {
        mapFade.color = c;
    }

    private IEnumerator DisableMapFade(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        mapFade.enabled = false;
    }

    private IEnumerator MapFadeOutIn(float time)
    {
        time /= 2;
        mapFade.enabled = true;
        Color fromColor = new Color(0, 0, 0, 0);
        Color toColor = new Color(0, 0, 0, 1);
        LeanTween.value(mapFade.gameObject, setFadeCallback, fromColor, toColor, time);
        yield return YieldInstructionCache.WaitForSeconds(time);
        fromColor = new Color(0, 0, 0, 1);
        toColor = new Color(0, 0, 0, 0);
        LeanTween.value(mapFade.gameObject, setFadeCallback, fromColor, toColor, time);
        yield return YieldInstructionCache.WaitForSeconds(time);
        mapFade.enabled = false;
    }

    public void AddTurn()
    {
        UseLantern();
        if(GameManager.Inst.Exploration.Satiety == 0)
        {
            float minus = GameManager.Inst.Exploration.player.MaxHP / 20;
            GameManager.Inst.Exploration.player.HP -= minus;
            playerUI.DrawHP();
        }
        else
            GameManager.Inst.Exploration.Satiety -= 5;
        GameManager.Inst.Exploration.Fatigue -= 5;
        playerUI.DrawSatiety();
        playerUI.DrawFatigue();
    }

    public void UseLantern()
    {
        GameManager.Inst.Exploration.Lantern -= 5;
        DrawLanternLight();
    }

    public void FillLantern()
    {
        GameManager.Inst.Exploration.Lantern += 50;
        DrawLanternLight();
    }

    private void DrawLanternLight()
    {
        switch (GameManager.Inst.Exploration.Light)
        {
            case LightType.Intensity:
                lantern.sprite = Resources.Load<Sprite>("Lantern/3");
                break;
            case LightType.Weakness:
                lantern.sprite = Resources.Load<Sprite>("Lantern/2");
                break;
            case LightType.Faint:
                lantern.sprite = Resources.Load<Sprite>("Lantern/1");
                break;
            case LightType.None:
                lantern.sprite = Resources.Load<Sprite>("Lantern/0");
                break;
        }
    }

    public void SetSkill(int index, int id)
    {
        // book skill
    }
    public bool SetArtifact(int index, int id)
    {
        for(int i = 1; i < 6; i++)
        {
            if (i != index && artifacts[i].ID == id)
            {
                return false;
            }
        }
        artifacts[index].SetArtifact(id);

        return true;
    }

    public void UseCameraController(bool use)
    {
        camController.enabled = use;
    }

    public void ShowCharactersInfo()
    {
        pimenu.gameObject.SetActive(false);
        camController.enabled = true;
        GameManager.Inst.player.Control = true;
    }

    public void ShowExplanArtifact(Vector3 position, int id)
    {
        explanation.gameObject.SetActive(true);
        SetExplanPopupPivot();
        explanation.transform.position = position;
        explanation.SetArtifact(id);
    }

    public void ShowExplanItem(Vector3 position, int id)
    {
        explanation.gameObject.SetActive(true);
        SetExplanPopupPivot();
        explanation.transform.position = position;
        explanation.SetItem(id);
    }

    private void SetExplanPopupPivot()
    {
        float width = Screen.width - Input.mousePosition.x;
        float height = Screen.height - Input.mousePosition.y; 
        if (width > 375)
        {
            if(height > 160)
            {
                Vector2 pivotCalculated = new Vector2(0, 1);
                explanPopup.rectTransform.pivot = pivotCalculated;
            }
            else
            {
                Vector2 pivotCalculated = new Vector2(0, 0);
                explanPopup.rectTransform.pivot = pivotCalculated;
            }
        }
        else
        {
            if (height > 160)
            {
                Vector2 pivotCalculated = new Vector2(1, 1);
                explanPopup.rectTransform.pivot = pivotCalculated;
            }
            else
            {
                Vector2 pivotCalculated = new Vector2(1, 0);
                explanPopup.rectTransform.pivot = pivotCalculated;
            }
        }
           

    }

    public void CloseExplan()
    {
        explanation.gameObject.SetActive(false);
    }

    private void RemoveAddListener()
    {
        messagePopup.onClick.RemoveAllListeners();
        textButton0.onClick.RemoveAllListeners();
        textButton1.onClick.RemoveAllListeners();
        textButton2.onClick.RemoveAllListeners();
        textButton3.onClick.RemoveAllListeners();
    }

    private void SetButtonTrue(int count)
    {
        if (count == 4)
        {
            text2.gameObject.SetActive(true);
            text3.gameObject.SetActive(true);
        }
        text0.gameObject.SetActive(true);
        text1.gameObject.SetActive(true);
    }

    private void SetButtonsFalse()
    {
        text0.gameObject.SetActive(false);
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);
    }
    private void SetTextYesNo()
    {
        text0.text = "예";
        text1.text = "아니요";
    }

    public void Notify(TileType type)
    {
        RemoveAddListener();
        messagePopup.gameObject.SetActive(true);
        messagePopup.enabled = false;
        switch (type)
        {
            case TileType.Chest:
                message.text = "보물 상자가 있습니다. 열어보시겠습니까?";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(OpenChest);
                textButton1.onClick.AddListener(CloseEvent);
                SetTextYesNo();
                break;
            case TileType.Boss:
                message.text = "층의 보스를 발견했습니다.";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(Fight);
                textButton1.onClick.AddListener(RunAway);
                text0.text = "싸운다.";
                text1.text = "도망친다.";
                break;
            case TileType.MiddleBoss:
                message.text = "중간 보스를 발견했습니다.";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(Fight);
                textButton1.onClick.AddListener(RunAway);
                text0.text = "싸운다.";
                text1.text = "도망친다.";
                break;
            case TileType.Monster:
                message.text = "몬스터를 발견했습니다.";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(Fight);
                textButton1.onClick.AddListener(RunAway);
                text0.text = "싸운다.";
                text1.text = "도망친다.";
                break;
            case TileType.PositiveEvent:
                message.text = "긍정적 이벤트";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(CloseEvent);
                textButton1.onClick.AddListener(CloseEvent);
                text0.text = "확인";
                text1.text = "확인";
                break;
            case TileType.NegativeEvent:
                message.text = "부정적 이벤트";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(CloseEvent);
                textButton1.onClick.AddListener(CloseEvent);
                text0.text = "확인";
                text1.text = "확인";
                break;
        }
    }

    public void CheckSearch()
    {
        RemoveAddListener();
        pimenu.gameObject.SetActive(false);

        if (mapManager.CheckSearched())
        {
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = true;
            messagePopup.onClick.AddListener(CloseMessagePopup);
            message.text = "이 이상의 탐색은 무의미하다.";
            SetButtonsFalse();
        }
        else
        {
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = false;
            message.text = "이곳을 탐색해볼까? (허기과 피로도가 5씩 소모됩니다.)";
            SetButtonTrue(2);
            textButton0.onClick.AddListener(Search);
            textButton1.onClick.AddListener(CloseMessagePopup);
            SetTextYesNo();
        }
    }

    public void Search()
    {
        RemoveAddListener();
        SearchEventType type = mapManager.Search();

        GameManager.Inst.Exploration.Satiety -= 5;
        GameManager.Inst.Exploration.Fatigue -= 5;
        playerUI.DrawSatiety();
        playerUI.DrawFatigue();

        switch (type)
        {
            case SearchEventType.None:
                messagePopup.gameObject.SetActive(true);
                messagePopup.enabled = true;
                messagePopup.onClick.AddListener(CloseMessagePopup);
                message.text = "아무것도 발견하지 못 했다.";
                SetButtonsFalse();
                break;
            case SearchEventType.FindSecrect:
                messagePopup.gameObject.SetActive(true);
                messagePopup.enabled = false;
                message.text = "벽에서 수상한 흔적을 발견했다. 벽을 부숴볼까? (폭탄이 한 개 소모됩니다.)";
                SetButtonTrue(2);
                textButton0.onClick.AddListener(BlownUp);
                textButton1.onClick.AddListener(CloseMessagePopup);
                SetTextYesNo();
                break;
            case SearchEventType.Chest:
                mapManager.SetChest();
                messagePopup.gameObject.SetActive(true);
                messagePopup.enabled = true;
                messagePopup.onClick.AddListener(CloseMessagePopup);
                message.text = "보물 상자를 발견했다!";
                SetButtonsFalse();
                break;
            case SearchEventType.Monster:
                GameManager.Inst.Exploration.SetSquadID(TileType.Monster);
                messagePopup.gameObject.SetActive(true);
                messagePopup.enabled = true;
                messagePopup.onClick.AddListener(Fight);
                message.text = "몬스터에게 습격당했다!";
                SetButtonsFalse();
                break;
        }
    }

    private void BlownUp()
    {
        if (GameManager.Inst.Exploration.player.invenory.GetItemAmount(5) > 0)
        {
            inventory.DeleteItem(5, 1);
            mapManager.BlownUp();
            mapManager.DeleteEvent();
            CloseMessagePopup();
        }
        else
        {
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = true;
            messagePopup.onClick.AddListener(CloseMessagePopup);
            message.text = "폭탄이 부족하다.";
            SetButtonsFalse();
        }
    }

    public void Campfire()
    {
        pimenu.gameObject.SetActive(false);
        RemoveAddListener();
        messagePopup.gameObject.SetActive(true);
        messagePopup.enabled = false;
        message.text = "여기서 쉬었다 갈까? (허기가 25 소모됩니다.)";
        SetButtonTrue(2);
        textButton0.onClick.AddListener(Rest);
        textButton1.onClick.AddListener(CloseMessagePopup);
        SetTextYesNo();
    }

    private void Rest()
    {
        bool attack = UnityEngine.Random.Range(1, 100) < 25 ? true : false;
        StartCoroutine(Resting(attack));
        messagePopup.gameObject.SetActive(false);
    }

    private IEnumerator Resting(bool attack)
    {
        mapManager.Campfire(true);
        GameManager.Inst.player.gameObject.SetActive(false);
        yield return YieldInstructionCache.WaitForSeconds(1);
        StartCoroutine(MapFadeOutIn(2));
        yield return YieldInstructionCache.WaitForSeconds(1f);
        mapManager.Campfire(false);
        GameManager.Inst.player.gameObject.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(1f);

        GameManager.Inst.Exploration.Satiety -= 25;
        GameManager.Inst.Exploration.player.HP += GameManager.Inst.Exploration.player.MaxHP * 0.5f;
        if (attack)
            GameManager.Inst.Exploration.Fatigue += 25;
        else
            GameManager.Inst.Exploration.Fatigue += 50;

        playerUI.DrawHP();
        playerUI.DrawSatiety();
        playerUI.DrawFatigue();

        if (attack)
        {
            RemoveAddListener();
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = true;
            messagePopup.onClick.AddListener(Fight);
            message.text = "몬스터의 습격이다!";
            SetButtonsFalse();
        }
        else
        {
            SetControlTrue();
        }
    }

    private void OpenChest()
    {
        messagePopup.gameObject.SetActive(false);
        chest.gameObject.SetActive(true);
        chest.SetPopup(mapManager.CurrentChest);
        mapManager.CurrentChest = ChestType.None;
    }

    public void ChangeChestPopupItem(int amount)
    {
        chest.SetPopupItem(amount);
    }

    public bool AddArtifact(int id)
    {
        return bubbleManager.Spawn(id);
    }

    public bool AddItem(Item newItem, out int remain)
    {
        return inventory.AddItem(newItem, out remain);
    }

    public bool SaleItem(int id, int amount)
    {
        return inventory.DeleteItem(id, amount);
    }

    public void UpdateInventory()
    {
        inventory.SetInventoryUI();
    }

    public void Fight()
    {
        messagePopup.gameObject.SetActive(false);
        canvas.sortingOrder = 1;
        fade.FadeIn(1f);
        StartCoroutine(WaitLoadScene(SceneName.FightScene, 1f));
    }

    public void Win()
    {
        mapManager.DeleteEvent();
        mapManager.MovePlayer(0.1f);
    }

    public void Loss()
    {
        mapManager.DeleteEvent();
        mapManager.MovePlayer(0.1f);
    }

    private void RunAway()
    {
        if(UnityEngine.Random.Range(0,100) < 30)
        {
            messagePopup.gameObject.SetActive(false);
            mapManager.DeleteMonster();
            mapManager.ReturnPlayerToPrevious();
        }
        else
        {
            RemoveAddListener();
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = true;
            messagePopup.onClick.AddListener(Fight);
            message.text = "도주에 실패했다!";
            SetButtonsFalse();
        }
    }

    public void ShowTextWhile(int id, int index)
    {
        string text;
        if(GameManager.Inst.GetScriptData(id, index, out text))
        {
            messagePopup.gameObject.SetActive(true);
            messagePopup.enabled = true;
            SetButtonsFalse();
            message.text = text;
            messagePopup.onClick.AddListener(delegate { ShowTextWhile(id, ++index); });
        }
        else
        {
           callBack();
        }
    }

    public void MeetMerchant()
    {
        if (!GameManager.Inst.PlayerData.DidMet(1)) // merchant id
        {
            GameManager.Inst.PlayerData.Meet(1);
            callBack = SpeakMerchant;
            ShowTextWhile(1, 0);
        }
        else
        {
            SpeakMerchant();
        }
    }

    public void SpeakMerchant()
    {
        messagePopup.gameObject.SetActive(true);
        messagePopup.enabled = false;
        if (GameManager.Inst.PlayerData.DidMet(1))
            message.text = "그래서, 어떡할 거야?";
        else
        {
            GameManager.Inst.PlayerData.Meet(1);
            message.text = "안녕, 또 만났네.";
        }

        SetButtonTrue(4);
        text0.text = "- 상점";
        text1.text = "- 다음 층으로";
        text2.text = "- 탈출";
        text3.text = "- 조금 있다가 다시 올게";
        textButton0.onClick.AddListener(OpenShop);
        textButton1.onClick.AddListener(() => { AppearPortal(Color.blue); });
        textButton2.onClick.AddListener(() => { AppearPortal(Color.red); });
        textButton3.onClick.AddListener(CloseMessagePopup);
    }

    private void OpenShop()
    {
        shop.gameObject.SetActive(true);
        messagePopup.gameObject.SetActive(false);
    }

    private void AppearPortal(Color color)
    {
        mapManager.AppearPortal(color);
        messagePopup.gameObject.SetActive(false);
        GameManager.Inst.StartCoroutine(GameManager.Inst.WaitAndQuitGame(2));
    }

    public void EndEvent()
    {
        mapManager.DeleteEvent();
        mapManager.MovePlayer(0.1f);
    }

    private void CloseEvent()
    {
        messagePopup.gameObject.SetActive(false);
        mapManager.DeleteEvent();
        mapManager.MovePlayer(0.1f);
    }

    private void CloseMessagePopup()
    {
        SetButtonsFalse();
        messagePopup.gameObject.SetActive(false);
        SetControlTrue();
    }

    public void OpenBook()
    {
        book.gameObject.SetActive(true);
        book.ShowFirst();
    }

    public void SetControlTrue()
    {
        camController.enabled = true;
        GameManager.Inst.player.Control = true;
    }

    public override void SetHP()
    {
        playerUI.DrawHP();
    }

    public void SetSatiety()
    {
        GameManager.Inst.Exploration.Satiety += 25;
        playerUI.DrawSatiety();
    }

    public void AppearMerchant()
    {
        mapManager.AppearPortal();
    }

    public void SetPearl(int value)
    {
        StartCoroutine(ChangePearl(value));
    }

    private IEnumerator ChangePearl(int value)
    {
        void SetCallback(Color c)
        {
            pearlPlus.color = c;
        }

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        int current = Int32.Parse(pearl.text);
        float deltaTime = 0;
        if (value != 0)
            deltaTime = 1 / Mathf.Abs(value);
        if (value > 0)
        {
            GameManager.Inst.Exploration.PlusPearl(value);
            pearlPlus.enabled = true;
            pearlPlus.transform.position = pearl.transform.position + new Vector3(50, -10, 0);
            pearlPlus.text = "+ " + value;
            pearlPlus.transform.LeanMove(pearl.transform.position + new Vector3(50, 0, 0), 1f);
            Color toColor = new Color(1, 1, 1, 0);
            LeanTween.value(pearlPlus.gameObject, SetCallback, Color.white, toColor, 1f);
            yield return YieldInstructionCache.WaitForSeconds(1f);
            pearlPlus.enabled = false;

            while (current != GameManager.Inst.Exploration.player.Pearl)
            {
                pearl.text = (current++).ToString();
                yield return YieldInstructionCache.WaitForSeconds(deltaTime);
            }
        }
        else
        {
            GameManager.Inst.Exploration.MinusPearl(-value);
            while (current != GameManager.Inst.Exploration.player.Pearl)
            {
                pearl.text = (current--).ToString();
                yield return YieldInstructionCache.WaitForSeconds(deltaTime);
            }
        }
        pearl.text = GameManager.Inst.Exploration.player.Pearl.ToString();
    }

    private IEnumerator SetEXP(int value)
    {
        void setCallback(float v)
        {
            exp.fillAmount = v;
        }

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        GameManager.Inst.player.Control = false;
        GameManager.Inst.Exploration.player.exp += value;
        if(GameManager.Inst.Exploration.player.exp >= GameManager.Inst.Exploration.player.level * 100)
        {
            StartCoroutine(WaitAddNotifyLogic((int)VictoryEvent.LevelUp, levelupEvent));
            float fromValue = exp.fillAmount;
            float toValue = 1f;
            LeanTween.value(exp.gameObject, setCallback, fromValue, toValue, 1f-fromValue);
            yield return YieldInstructionCache.WaitForSeconds(1-fromValue);

            GameManager.Inst.Exploration.player.exp -= GameManager.Inst.Exploration.player.level * 100;
            GameManager.Inst.Exploration.player.level += 1;
            level.text = "Lv." + GameManager.Inst.Exploration.player.level;

            fromValue = 0f;
            toValue = (float)GameManager.Inst.Exploration.player.exp / (GameManager.Inst.Exploration.player.level * 100);
            LeanTween.value(exp.gameObject, setCallback, fromValue, toValue, 1-toValue);
            yield return YieldInstructionCache.WaitForSeconds(1-toValue);
        }
        else
        {
            float fromValue = exp.fillAmount;
            float toValue = (float)GameManager.Inst.Exploration.player.exp / (GameManager.Inst.Exploration.player.level * 100);
            LeanTween.value(exp.gameObject, setCallback, fromValue, toValue, 1f);
            yield return YieldInstructionCache.WaitForSeconds(1);
        }
        GameManager.Inst.player.Control = true;
    }

    public void SetFull(bool isItem)
    {
        full.gameObject.SetActive(true);
        if (isItem) 
        {
            full.SetFullItem();
        }
        else
        {
            full.SetFullArtifact();
        }
    }

    public void SetArtifactPopup(bool isEquip, Bubble bubble)
    {
        clickArtifact = bubble;
        artifactPopup.gameObject.SetActive(true);
        if (isEquip)
        {
            artifactPopup.SetEquip();
        }
        else
        {
            artifactPopup.SetDestroy();
        }
    }

    public void EquipArtifact()
    {
        clickArtifact.Equip();
        clickArtifact = null;
    }

    public void DestroyArtifact()
    {
        clickArtifact.Despown();
        clickArtifact = null;
    }

    public void MoveArtifactToPreposition()
    {
        clickArtifact.GoToPreposition();
    }

    private void GetLoot()
    {
        List<Entity_SquadMember> squad = new List<Entity_SquadMember>();
        GameManager.Inst.GetSquadMemberDatas(GameManager.Inst.Exploration.GetSquad().ID, out squad);
        List<Entity_Monster> monsters = new List<Entity_Monster>();
        foreach (Entity_SquadMember member in squad)
        {
            Entity_Monster monster;
            GameManager.Inst.GetMonsterData(member.MonsterID, out monster);
            monsters.Add(monster);
        }

        List<ItemArtifactData> loots = new List<ItemArtifactData>();
        ItemArtifactData data;
        foreach(Entity_Monster monster in monsters)
        {
            if (monster.ID < 700) // normal monster
            {
                bool percent = UnityEngine.Random.Range(0, 100) < 10 ? true : false;
                if (percent) // rune
                {
                    data = new ItemArtifactData(9, true, GameManager.Inst.GetRuneCount());
                    loots.Add(data);
                    continue;
                }
                percent = UnityEngine.Random.Range(0, 100) < 5 ? true : false;
                if (percent) // essence
                {
                    data = new ItemArtifactData(GameManager.Inst.GetEssence(), true, 1);
                    loots.Add(data);
                    continue;
                }
                percent = UnityEngine.Random.Range(0,100) < monster.DropPercent ? true : false;
                if (percent) // exchange
                {
                    data = new ItemArtifactData(monster.DropItem, true, 1);
                    loots.Add(data);
                }
            }
            else if(monster.ID < 900) // middle boss
            {
                bool artifact = UnityEngine.Random.Range(0, 100) < 50 ? true : false;
                if (artifact)
                {
                    data = new ItemArtifactData(GameManager.Inst.GetRandomMaterialOrItemArtifact(), false, 1);
                    loots.Add(data);
                    continue;
                }
                data = new ItemArtifactData(monster.DropItem, true, 1);
                loots.Add(data);
            }
            else // boss
            {
                data = new ItemArtifactData(GameManager.Inst.GetRandomProductArtifact(), false, 1);
                loots.Add(data);
            }
        }
        

        lootpopup.gameObject.SetActive(true);
        foreach(var loot in loots)
        {
            if (loot.isItem)
            {
                lootpopup.SetItem(loot.id, loot.amount);
            }
            else
            {
                lootpopup.SetArtifact(loot.id);
            }
        }
        
        if (loots.Count == 0)
        {
            lootpopup.gameObject.SetActive(false);
            victory.StartVictoryLogic();
        }
    }

    private void LevelUp()
    {
        levelup.gameObject.SetActive(true);
    }

    private void StartMerchantEvent()
    {
        mapManager.StartMerchantEvent();    
    }

    public void StartNextVictoryLogic()
    {
        victory.StartVictoryLogic();
    }

    private IEnumerator WaitAddNotifyLogic(int key, NotifyLogic notify)
    {
        while(!victory.AddNotifyLogic(key, notify))
        {
            yield return null;
        }
    }
}

