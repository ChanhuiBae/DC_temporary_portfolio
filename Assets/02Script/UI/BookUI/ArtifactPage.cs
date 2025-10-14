using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactPage : PageUI
{
    private Image bubble;
    private Image icon;
    private TextMeshProUGUI name;
    private TextMeshProUGUI description;
    private Image bubble1;
    private Image bubble2;
    private Image icon1;
    private Image icon2;

    private void Awake()
    {
        Transform t = transform.Find("Bubble");
        if (t == null || !t.TryGetComponent<Image>(out bubble))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
        t = transform.Find("Icon");
        if (t == null || !t.TryGetComponent<Image>(out icon))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
        t = transform.Find("Name");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("ArtifactPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Description");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out description))
        {
            Debug.Log("ArtifactPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Bubble1");
        if (t == null || !t.TryGetComponent<Image>(out bubble1))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
        if (t == null || !t.GetChild(0).TryGetComponent<Image>(out icon1))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
        t = transform.Find("Bubble2");
        if (t == null || !t.TryGetComponent<Image>(out bubble2))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
        if (t == null || !t.GetChild(0).TryGetComponent<Image>(out icon2))
        {
            Debug.Log("ArtifactPage - Awake - Image");
        }
    }

    public void SetPage(Entity_Artifact artifact)
    {
        icon.sprite = Resources.Load<Sprite>("Artifact/" +  artifact.Icon);
        name.text = artifact.Name;
        description.text = artifact.Explanation;
        bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble" + artifact.Grade.ToString());
        if(artifact.Grade > 1)
        {
            bubble1.gameObject.SetActive(true);
            bubble1.sprite = Resources.Load<Sprite>("Bubble/Bubble" + (artifact.Grade - 1));
            bubble2.gameObject.SetActive(true);
            bubble2.sprite = Resources.Load<Sprite>("Bubble/Bubble" + (artifact.Grade - 1));

            Entity_AritifactCombination combination;
            GameManager.Inst.GetArtifactCombination(artifact.ID, out combination);
            Entity_Artifact data;
            GameManager.Inst.GetArtifactData(combination.First, out data);
            icon1.sprite = Resources.Load<Sprite>("Artifact/" + data.Icon);
            GameManager.Inst.GetArtifactData(combination.Second, out data);
            icon2.sprite = Resources.Load<Sprite>("Artifact/" + data.Icon);
        }
        else
        {
            bubble1.gameObject.SetActive(false);
            bubble2.gameObject.SetActive(false);

        }
    }
}
