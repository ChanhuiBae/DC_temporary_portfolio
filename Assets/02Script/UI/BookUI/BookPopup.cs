using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPopup : MonoBehaviour
{
    private Image icon;
    private TextMeshProUGUI name;
    private TextMeshProUGUI explain;
    private ListColorUI stars;
    private Image attackArea;

    private void Awake()
    {
        GameObject ob = transform.Find("Name").gameObject;
        if(ob == null || !ob.TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("BookPopup - Awake - TextMeshProUGUI");
        }
        ob = transform.Find("Text").gameObject;
        if(ob == null || !ob.TryGetComponent<TextMeshProUGUI>(out explain))
        {
            Debug.Log("BookPopup - Awake - TextMeshProUGUI");
        }
        Transform ui = transform.Find("Star");
        if (ui == null || !ui.TryGetComponent<ListColorUI>(out stars))
        {
            Debug.Log("BookPopup - Awake - ListColorUI");
        }
        ui = transform.Find("AttackArea");
        if (ui == null || !ui.TryGetComponent<Image>(out attackArea))
        {
            Debug.Log("BookPopup - Awake - Image");
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetWeaponPopup(int weapon)
    {
        stars.gameObject.SetActive(false);
        attackArea.gameObject.SetActive(false);
        switch (weapon)
        {
            case 0:
                name.text = "지팡이";
                explain.text = "";
                break;
            case 1:
                name.text = "모자";
                explain.text = "";
                break;
            case 2:
                name.text = "로프";
                explain.text = "";
                break;
        }
    }
    public void SetSkillPopup(Entity_Skill skill)
    {
        stars.gameObject.SetActive(true);
        name.text = skill.Name;
        explain.text = skill.Exploration;
        stars.SetStars(1); // LEVEL
        if (skill.AttackArea == "None")
        {
            attackArea.gameObject.SetActive(false);
        }
        else
        {
            attackArea.gameObject.SetActive(true);
            attackArea.sprite = Resources.Load<Sprite>("AttackArea/" + skill.AttackArea);
        }
    }

    public void SetPartnerPopup(Entity_Skill skill)
    {
        stars.gameObject.SetActive(false);
        attackArea.gameObject.SetActive(true);
        name.text = skill.Name;
        explain.text = skill.Exploration;
        attackArea.sprite = Resources.Load<Sprite>("AttackArea/" + skill.AttackArea);
    }
}
