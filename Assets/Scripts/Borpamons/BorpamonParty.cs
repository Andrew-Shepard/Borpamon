using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BorpamonParty : MonoBehaviour
{
    [SerializeField] List<Borpamon> borpamons;

    private void Start()
    {
        foreach (var Borpamon in borpamons)
        {
            Borpamon.Init();
        }
    }

    public Borpamon GetHealthyBorpamon()
    {
        return borpamons.Where(x => x.HP > 0).FirstOrDefault(); 
        // Where loops though the list of borpamons and returns a list that satisfies
        //HP>0, FirstorDefault only returns the first result.
    }
}
