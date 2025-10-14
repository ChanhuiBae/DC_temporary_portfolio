using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoice : MonoBehaviour
{
    private LevelUpPopup levelup;
    private Button button;
    private TextMeshProUGUI name;
    private Image icon;
    private ListColorUI star;
    private ListColorUI attackArea;
    private Image textback;
    private TextMeshProUGUI description;

    private void Awake()
    {
        if(!transform.parent.TryGetComponent<LevelUpPopup>(out levelup))
        {
            Debug.Log("SkillChoice - Awake - LevelUpPopup");
        }
        if(!TryGetComponent<Button>(out button))
        {
            Debug.Log("SkillChoice - Awake - Button");
        }
        GameObject obj = transform.Find("Name").gameObject;
        if(obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("SkillChoice - Awake - TextMeshProUGUI");
        }
        obj = transform.Find("Icon").gameObject;
        if (obj == null || !obj.TryGetComponent<Image>(out icon))
        {
            Debug.Log("SkillChoice - Awake - Image");
        }
        obj = transform.Find("Star").gameObject;
        if (obj == null || !obj.TryGetComponent<ListColorUI>(out star))
        {
            Debug.Log("SkillChoice - Awake - ListColorUI");
        }
        obj = transform.Find("AttackArea").gameObject;
        if (obj == null || !obj.TryGetComponent<ListColorUI>(out attackArea))
        {
            Debug.Log("SkillChoice - Awake - ListColorUI");
        }
        obj = transform.Find("TextBack").gameObject;
        if (obj == null || !obj.TryGetComponent<Image>(out textback))
        {
            Debug.Log("SkillChoice - Awake - Image");
        }
        obj = obj.transform.GetChild(0).gameObject;
        if (obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out description))
        {
            Debug.Log("SkillChoice - Awake - TextMeshProUGUI");
        }
    }

    private void OnEnable()
    {
        button.onClick.AddListener(SetSkill);
    }

    public void SetSkill()
    {
        //todo: Set skill
        levelup.Close();
    }
}
