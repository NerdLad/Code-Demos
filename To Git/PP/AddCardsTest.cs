using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsTest : MonoBehaviour {

    public void AddCrabCardByName(string cardName){
        foreach (string card in PlayData.collectedCrabCards){
            Debug.Log("PlayData Crab Card: " + card);
            if (card == cardName) return;
        }
        PlayData.collectedCrabCards.Add(cardName);
    }

    public void AddDolphinCardByName(string cardName)
    {
        foreach (string card in PlayData.collectedDolphinCards)
        {
            Debug.Log("PlayData Dolpin Card: " + card);
            if (card == cardName) return;
        }
        PlayData.collectedDolphinCards.Add(cardName);
    }

    public void DeleteObject(GameObject obj){
        Destroy(obj);
    }
}
