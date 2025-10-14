using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationPopup : MonoBehaviour
{
    private Image icon;
    private TextMeshProUGUI explanation;
    private TextMeshProUGUI name;

    private void Awake()
    {
        if(!transform.GetChild(1).TryGetComponent<Image>(out icon))
        {
            Debug.Log("ExplanationPopup - Awake - Image");
        }
        if(!transform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out explanation))
        {
            Debug.Log("ExplanationPopup - Awake - TextMeshProUGUI");
        }
        if(!transform.GetChild(3).TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("ExplanationPopup - Awake - TextMeshProUGUI");
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetArtifact(int id)
    {
        Entity_Artifact artifact;
        GameManager.Inst.GetArtifactData(id, out artifact);
        icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
        name.text = artifact.Name;
        explanation.text = artifact.Effect;
    }

    public void SetItem(int id)
    {
        Entity_Item item;
        GameManager.Inst.GetItemData(id, out item);
        icon.sprite = Resources.Load<Sprite>("Item/"+item.Icon);
        name.text = item.Name;
        explanation.text = item.Explanation;
    }
}
