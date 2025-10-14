using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerPage : PageUI
{
    private TextMeshProUGUI atk;
    private TextMeshProUGUI def;
    private TextMeshProUGUI speed;
    private TextMeshProUGUI critical;
    private TextMeshProUGUI criticalDamage;
    private TextMeshProUGUI fire;
    private TextMeshProUGUI water;
    private TextMeshProUGUI earth;
    private TextMeshProUGUI thunder;
    private TextMeshProUGUI time;
    private TextMeshProUGUI pearl;
    private TextMeshProUGUI allPearl;
    private EventTrigger weapon;
    private EventTrigger hat;
    private EventTrigger cloth;

    private EventTrigger.Entry enterWeapon;
    private EventTrigger.Entry enterHat;
    private EventTrigger.Entry enterCloth;
    private EventTrigger.Entry exit;

    private BookPopup popup;

    private void Awake()
    {
        Transform t = transform.Find("ATK");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out atk))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("DEF");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out def))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Speed");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out speed))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Critical");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out critical))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("CriDamage");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out criticalDamage))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Fire");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out fire))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Water");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out water))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Earth");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out earth))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Thunder");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out thunder))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Time");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out time))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("CurrentPearl");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out pearl))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("AllPearl");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out allPearl))
        {
            Debug.Log("PlayerPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Weapon");
        if (t == null || !t.TryGetComponent<EventTrigger>(out weapon))
        {
            Debug.Log("PlayerPage - Awake - EventTrigger");
        }
        t = transform.Find("Hat");
        if (t == null || !t.TryGetComponent<EventTrigger>(out hat))
        {
            Debug.Log("PlayerPage - Awake - EventTrigger");
        }
        t = transform.Find("Cloth");
        if (t == null || !t.TryGetComponent<EventTrigger>(out cloth))
        {
            Debug.Log("PlayerPage - Awake - EventTrigger");
        }
        GameObject obj = GameObject.Find("BookPopup");
        if (obj == null || !obj.TryGetComponent<BookPopup>(out popup))
        {
            Debug.Log("PlayerPage - Awake - BookPopup");
        }
    }

    private void Start()
    {
        enterWeapon = new EventTrigger.Entry();
        enterWeapon.eventID = EventTriggerType.PointerEnter;
        enterWeapon.callback.AddListener(delegate { ShowWeapon(0); });
        weapon.triggers.Add(enterWeapon);

        enterHat = new EventTrigger.Entry();
        enterHat.eventID = EventTriggerType.PointerEnter;
        enterHat.callback.AddListener(delegate { ShowWeapon(1); });
        hat.triggers.Add(enterHat); 

        enterCloth = new EventTrigger.Entry();
        enterCloth.eventID = EventTriggerType.PointerEnter;
        enterCloth.callback.AddListener(delegate { ShowWeapon(2); });
        cloth.triggers.Add(enterCloth);

        exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener(delegate { ClosePopup(); });
        weapon.triggers.Add(exit);
        hat.triggers.Add(exit);
        cloth.triggers.Add(exit);
    }

    public void SetPage()
    {
        atk.text = GameManager.Inst.Exploration.player.ATK.ToString();
        def.text = GameManager.Inst.Exploration.player.DEF.ToString();
        speed.text = GameManager.Inst.Exploration.cast_speed.ToString();
        critical.text = GameManager.Inst.Exploration.player.critical.ToString();
        criticalDamage.text = GameManager.Inst.Exploration.player.criticalDamage.ToString();
        fire.text = GameManager.Inst.Exploration.player.Fire.ToString();
        water.text = GameManager.Inst.Exploration.player.Water.ToString();
        earth.text = GameManager.Inst.Exploration.player.Earth.ToString();
        thunder.text = GameManager.Inst.Exploration.player.Thunder.ToString();
        time.text = "00:00";
        pearl.text = GameManager.Inst.Exploration.player.Pearl.ToString();
        allPearl.text = GameManager.Inst.Exploration.AllPearl.ToString();
        popup.gameObject.SetActive(false);
    }

    private void ShowWeapon(int weapon)
    {
        popup.gameObject.SetActive(true);
        popup.SetWeaponPopup(weapon);
    }

    private void ClosePopup()
    {
        popup.gameObject.SetActive(false);
    }

}
