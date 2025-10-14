using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private Image hp;
    private TextMeshProUGUI hpValue;
    private Image satiety;
    private TextMeshProUGUI satietyValue;
    private Image fatigue;
    private TextMeshProUGUI fatigueValue;

    [SerializeField]
    private SpriteLibrary sl;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Image>(out hp))
        {

        }
        if(!transform.GetChild(1).TryGetComponent<Image>(out satiety))
        {

        }
        if(!transform.GetChild(2).TryGetComponent<Image>(out fatigue))
        {

        }
        if(!transform.GetChild(0).GetChild(2).TryGetComponent<TextMeshProUGUI>(out hpValue))
        {

        }
        if (!transform.GetChild(1).GetChild(2).TryGetComponent<TextMeshProUGUI>(out satietyValue))
        {

        }
        if (!transform.GetChild(2).GetChild(2).TryGetComponent<TextMeshProUGUI>(out fatigueValue))
        {

        }
    }

    private void Start()
    {
        StartCoroutine(WaitCharactersData());
    }

    private IEnumerator WaitCharactersData()
    {
        while(GameManager.Inst.Exploration == null)
        {
            yield return null;
        }
        DrawHP();
        DrawSatiety();
        DrawFatigue();
    }

    public void DrawHP()
    {
        hp.fillAmount = GameManager.Inst.Exploration.player.HP / GameManager.Inst.Exploration.player.MaxHP;
        hpValue.text = (int)GameManager.Inst.Exploration.player.HP + "/" + GameManager.Inst.Exploration.player.MaxHP;
    }

    public void DrawSatiety()
    {
        satiety.fillAmount = GameManager.Inst.Exploration.Satiety * 0.01f;
        satietyValue.text = GameManager.Inst.Exploration.Satiety + "%";
    }

    public void DrawFatigue()
    {
        fatigue.fillAmount = GameManager.Inst.Exploration.Fatigue * 0.01f;
        fatigueValue.text = GameManager.Inst.Exploration.Fatigue + "%";
    }

}
