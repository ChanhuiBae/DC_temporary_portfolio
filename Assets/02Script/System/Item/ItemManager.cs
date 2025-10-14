using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool UseItem(int id)
    {
        if (GameManager.Inst.Exploration.player.invenory.GetItemAmount(id) > 0)
        {
            GameManager.Inst.Exploration.player.invenory.DeleteItemAmount(id, 1);
            switch (id)
            {
                case 1:
                    UsePotion();
                    return true;
                case 2:
                    UseFood();
                    return true;
                case 3:
                    UseOil();
                    return true;
            }
        }
        return false;
    }
    public void UsePotion()
    {
        GameManager.Inst.eventManager.PostNotification(GameEventType.PotionUsed);
        GameManager.Inst.Exploration.player.HP += GameManager.Inst.Exploration.player.MaxHP * 0.3f;
    }

    public void UseFood()
    {
        ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
        manager.SetSatiety();
    }

    public void UseOil()
    {
        ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
        manager.FillLantern();
    }
}
