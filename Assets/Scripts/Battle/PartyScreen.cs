using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Borpamon> borpamons)
    {
        for(int i = 0; i< memberSlots.Length; i++)
        {
            if (i < borpamons.Count) // only show the slot if there is a party member in the list
            {
                memberSlots[i].SetData(borpamons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }
        messageText.text = "Choose a Pokemon";
    }
}
