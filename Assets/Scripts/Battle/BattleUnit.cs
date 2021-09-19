using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] BorpamonBase borpamon_base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Borpamon Borpamon { get; set; }
    public void Setup()
    {
        Borpamon = new Borpamon(borpamon_base, level);
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Borpamon.Borpamon_base.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Borpamon.Borpamon_base.FrontSprite;
        }
    }
}
