using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(Borpamon borpamon)
    {
        nameText.text = borpamon.Borpamon_base.Name;
        levelText.text = "Lvl " + borpamon.Level;
        hpBar.SetHP((float) borpamon.HP / borpamon.MaxHp);
    }
}
