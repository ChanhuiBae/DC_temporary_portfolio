using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactFight : MonoBehaviour
{
    private int id;
    public int ID
    {
        get => id;
    }
    private Image icon;
    private TextMeshProUGUI value;

    private void Awake()
    {
        if(!TryGetComponent<Image>(out icon))
        {
            Debug.Log("ArtifactFight - Awake - Image");
        }
        if (!transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out value))
        {
            Debug.Log("ArtifactFight - Awake - Image");
        }
        icon.enabled = false;
        value.enabled = false;
    }

    public void SetArtifact(int id)
    {
        this.id = id;
        Entity_Artifact artifact;
        GameManager.Inst.GetArtifactData(id, out artifact);
        icon.enabled = true;
        icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
    }

    public void SetValue(int value)
    {
        if(this.value != null)
        {
            this.value.enabled = true;
            this.value.text = value.ToString();
        }
    }

    public string GetValue()
    {
        return value.ToString();
    }
}
