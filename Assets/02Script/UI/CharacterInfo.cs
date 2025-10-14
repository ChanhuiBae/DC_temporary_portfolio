using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    private Image hp;
    private Image characterImage;
    private TextMeshProUGUI name;
    private TextMeshProUGUI level;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Image>(out hp))
        {
            Debug.Log("CharacterInfo - Awake - Image");
        }
        if (!transform.GetChild(1).GetChild(0).TryGetComponent<Image>(out characterImage))
        {
            Debug.Log("CharacterInfo - Awake - Image");
        }
        if (!transform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("CharacterInfo - Awake - TextMeshProUGUI");
        }
        if (!transform.GetChild(3).TryGetComponent<TextMeshProUGUI>(out level))
        {
            Debug.Log("CharacterInfo - Awake - TextMeshProUGUI");
        }
    }

    public void SetHP(float fill)
    {
        hp.fillAmount = fill;
    }

    public void SetCharacterImage(Sprite image)
    {
        characterImage.sprite = image;
    }

    public void SetName(string name)
    {
        this.name.text = name;
    }

    public void SetLevel(int level)
    {
        this.level.text = "Lv" + level;
    }

    public Vector3 GetPosition()
    {
        return characterImage.transform.position;
    }

    public void SetBoss()
    {
        Image thisImage = GetComponent<Image>();
        thisImage.rectTransform.sizeDelta = new Vector2(thisImage.rectTransform.sizeDelta.x * 2, thisImage.rectTransform.sizeDelta.y);
        hp.rectTransform.sizeDelta = new Vector2(hp.rectTransform.sizeDelta.x * 2, hp.rectTransform.sizeDelta.y);
    }
}
