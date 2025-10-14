using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactPopup : MonoBehaviour
{
    private TextMeshProUGUI main;
    private TextMeshProUGUI sub;
    private Button yes;
    private Button no;

    private ExplorationManager manager;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out main))
        {
            Debug.Log("ArtifactPopup - Awake - TextMeshProUGUI");
        }
        if (!transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out sub))
        {
            Debug.Log("ArtifactPopup - Awake - TextMeshProUGUI");
        }
        if (!transform.GetChild(2).TryGetComponent<Button>(out yes))
        {
            Debug.Log("ArtifactPopup - Awake - Button");
        }
        if (!transform.GetChild(3).TryGetComponent<Button>(out no))
        {
            Debug.Log("ArtifactPopup - Awake - Button");
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        no.onClick.AddListener(No);
        gameObject.SetActive(false);
    }

    public void SetEquip()
    {
        main.text = "아티팩트를 장착하시겠습니까?";
        sub.text = "기존에 장착된 아티팩트는 파괴됩니다.";
        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(Equip);
    }

    public void SetDestroy()
    {
        main.text = "해당 아티팩트를 파괴하시겠습니까?";
        sub.text = "파괴된 아티팩트는 되돌릴 수 없습니다.";
        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(Destroy);
    }

    private void No()
    {
        manager.MoveArtifactToPreposition();
        gameObject.SetActive(false);
    }

    private void Equip()
    {
        manager.EquipArtifact();
        gameObject.SetActive(false);
    }

    private void Destroy()
    {
        manager.DestroyArtifact();
        gameObject.SetActive(false);
    }
}
