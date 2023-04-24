using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterStatus : MonoBehaviour
{

    public int cardsInCrabGame = 4;
    public int cardsInTurtleGame = 4;
    public int cardsInDolphinGame = 4;

    public Card[] cardOptions = new Card[12];
    public Card[] playerCards = new Card[12];
    public Card[] turtleCards = new Card[4];
    public Card[] crabCards = new Card[4];
    public Card[] dolphinCards = new Card[4];

    public List<Encounter> encounters;
    public List<Encounter> introEncs;
    public List<Encounter> outroEncs;

    // Use this for initialization
    void Start(){
        if (!PlayData.cardsJumbled){
            JumbleCards();
            PlayData.cardsJumbled = true;
        }

        if(!PlayData.isInitialized){
            ResetPlayData();
            SetDataEncounters();
            SetUpCards();
            PlayData.SetCardsPerMinigame(cardsInCrabGame, cardsInTurtleGame, cardsInDolphinGame);
            PlayData.MarkInitialized(true);
        }
    }

    public void ResetPlayData(){
        PlayData.ClearAllEncounters();
        PlayData.ClearPlayerCards();
        PlayData.ClearCardOptions();
        PlayData.MarkInitialized(false);
    }

    public void SetUpCards(){
        foreach(Card card in cardOptions){
            PlayData.AddCardToCardOptions(card);
        }
    }

    public void SetDataEncounters(){
        foreach (Encounter enc in encounters){
            PlayData.AddEncounter(enc);
        }
        foreach (Encounter enc in introEncs)
        {
            PlayData.AddIntroEncounter(enc);
        }
        foreach (Encounter enc in outroEncs)
        {
            PlayData.AddOutroEncounter(enc);
        }
    }

    public void JumbleCards()
    {
        List<Card> unjumbledList = new List<Card>();
        List<Card> jumbledList = new List<Card>();

        int i = 0;
        while (i < cardOptions.Length){
            unjumbledList.Add(cardOptions[i]);
            i++;
        }

        int randomValue = 0;
        while (unjumbledList.Count > 0){
            randomValue = Random.Range(0, unjumbledList.Count);
            //Debug.Log("List length is: " + unjumbledList.Count + " and Random value is: " + randomValue);
            jumbledList.Add(unjumbledList[randomValue]);
            unjumbledList.Remove(unjumbledList[randomValue]);
        }

        i = 0;
        while (i < cardOptions.Length){
            cardOptions[i] = jumbledList[i];
            i++;
        }

    }
}
