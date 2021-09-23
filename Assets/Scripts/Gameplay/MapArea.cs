using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Borpamon> wildBorpamons;

    public Borpamon GetRandomWildBorpamon()
    {
        var wildBorpamon =  wildBorpamons[Random.Range(0, wildBorpamons.Count)];
        wildBorpamon.Init();
        return wildBorpamon;
    }

    
}
