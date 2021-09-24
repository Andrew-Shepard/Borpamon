using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image borpamonImage;

    Borpamon _borpamon;
    public void SetData(Borpamon borpamon)
    {
        _borpamon = borpamon;

        nameText.text = borpamon.Base.Name;
        levelText.text = "Lvl " + borpamon.Level;
        borpamonImage.sprite = borpamon.Base.FrontSprite;
        hpBar.SetHP((float)borpamon.HP / borpamon.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_borpamon.HP / _borpamon.MaxHp);
    }
}
