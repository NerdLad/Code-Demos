using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayData
{
    //Script for holding all of the data between scenes

    //lists all of the values this script holds
    private static bool runStartMenu = true;
    public static bool runCredits = false;
    //list for holding the cards
    public static List<string> collectedCrabCards = new List<string>();
    public static List<string> collectedTurtleCards = new List<string>();
    public static List<string> collectedDolphinCards = new List<string>();


    public static Card[] cardOptions = new Card[12];
    public static Card[] playerCards = new Card[12];

    public static List<Encounter> incompleteEncounters = new List<Encounter>();
    public static List<Encounter> completedEncounters = new List<Encounter>();

    public static List<Encounter> incompleteIntroEncs = new List<Encounter>();
    public static List<Encounter> completedIntroEncs = new List<Encounter>();

    public static List<Encounter> incompleteOutroEncs = new List<Encounter>();
    public static List<Encounter> completedOutroEncs = new List<Encounter>();

    public static bool isInitialized = false; //If not initialized, the 

    public static bool isSoundOn = true;

    public static int cardsInCrabGame = 4;
    public static int cardsInTurtleGame = 4;
    public static int cardsInDolphinGame = 4;

    public static bool cardsJumbled = false;

    public static bool[] dolphinLevelsComplete = new bool[4];

    #region General Functions
    //checks whether to show the startmenu upon loading the menu scene
    public static bool RunStartMenu
    {
        get{return runStartMenu;}
        set{runStartMenu = value;}
    }

    public static void MarkInitialized(bool isInitialized){
        PlayData.isInitialized = isInitialized;
    }

    public static bool SoundCheck {
        get { return isSoundOn; }
        set { isSoundOn = value; }
    }

    #endregion

    #region Handle Minigame Cards
    //holds the card values for each minigame
    public static int GetDolphinCardsCount
    {
        get{
            return collectedDolphinCards.Count;
        }

    }
    public static int GetTurtleCardsCount
    {
        get{
            return collectedTurtleCards.Count;
        }

    }
    public static int GetCrabCardsCount
    {
        get{
            return collectedCrabCards.Count;
        }
    }

    public static void SetCardsPerMinigame(int crabCards, int turtleCards, int dolphinCards){
        cardsInCrabGame = crabCards;
        cardsInTurtleGame = turtleCards;
        cardsInDolphinGame = dolphinCards;
    }

    public static void ResetDolphinGameLevels()
    {
        int i = 0;
        while (i < dolphinLevelsComplete.Length)
        {
            dolphinLevelsComplete[i] = false;
            i++;
        }
    }

    public static void SetDolphinLevelComplete(int levelNumber, bool isComplete)
    {
        dolphinLevelsComplete[levelNumber] = isComplete;
    }

    #endregion

    #region Handle Cards

    public static Card GetCardOptions(int index)
    {
        return cardOptions[index];
    }

    public static Card GetPlayerCards(int index){
        UpdateCards();
        return playerCards[index];
    }

    public static void UpdateCards()
    {
        ClearPlayerCards();

        int i = 0;
        int j = 0;

        while (i < GetCrabCardsCount)
        {
            if (i < cardsInCrabGame){
                AddCardToPlayerCards(cardOptions[j]);
            }
            i++;
            j++;
        }

        i = 0;
        while (i < GetTurtleCardsCount)
        {
            if (i < cardsInTurtleGame)
            {
                AddCardToPlayerCards(cardOptions[j]);
            }
            i++;
            j++;
        }

        i = 0;
        while (i < GetDolphinCardsCount)
        {
            if (i < cardsInDolphinGame)
            {
                AddCardToPlayerCards(cardOptions[j]);
            }
            i++;
            j++;
        }
    }

    public static void AddCardToPlayerCards(Card newCard)
    {
        //First check if the player already has that card.
        int i = 0;
        while (i < playerCards.Length){
            if (playerCards[i] == newCard) {
                Debug.LogWarning("Player Cards already contains that card!");
                return; 
            }
            i++;
        }

        //if the player doesn't have the card, find the newest empty slot and add it there.
        i = 0;
        bool foundASlot = false;
        while (i < playerCards.Length)
        {
            if (playerCards[i] == null) {foundASlot = true; playerCards[i] = newCard; break; }
            i++;
        }
        if(!foundASlot){
            Debug.LogWarning("Can't find an open slot to add card to!");
        }
    }

    public static void AddCardToCardOptions(Card newCard)
    {
        //First check if the player already has that card.
        int i = 0;
        while (i < cardOptions.Length){
            if (cardOptions[i] == newCard){
                Debug.LogWarning("Card Options already contains that card!");
                return;
            }
            i++;
        }

        //if the player doesn't have the card, find the newest empty slot and add it there.
        i = 0;
        bool foundASlot = false;
        while (i < cardOptions.Length){
            if (cardOptions[i] == null) { foundASlot = true; cardOptions[i] = newCard; break; }
            i++;
        }
        if (!foundASlot){
           Debug.LogWarning("Can't find an open slot to add card to!");
        }
    }

    public static void ClearPlayerCards(){
        int i = 0;
        while (i < playerCards.Length){
            playerCards[i] = null;
            i++;
        }
    }

    public static void ClearCardOptions(){
        int i = 0;
        while (i < cardOptions.Length){
            cardOptions[i] = null;
            i++;
        }
    }

    #endregion

    #region Handle Encounters

    public static Encounter GetEncounters(int index){
        return incompleteEncounters[index];
    }

    public static Encounter GetIntroEncs(int index){
        return incompleteIntroEncs[index];
    }

    public static Encounter GetOutroEncs(int index){
        return incompleteOutroEncs[index];
    }

    public static void CompleteEncounter(Encounter encounter){
        if (incompleteEncounters.Contains(encounter)){
            incompleteEncounters.Remove(encounter);
            completedEncounters.Add(encounter);
        }

        if (incompleteIntroEncs.Contains(encounter)){
            incompleteIntroEncs.Remove(encounter);
            completedIntroEncs.Add(encounter);
        }

        if (incompleteOutroEncs.Contains(encounter)){
            incompleteOutroEncs.Remove(encounter);
            completedOutroEncs.Add(encounter);
        }
    }

    public static void AddEncounter(Encounter newEnc){
        //First check if the player already has that card.
        int i = 0;
        while (i < incompleteEncounters.Count)
        {
            if (incompleteEncounters[i] == newEnc)
            {
                Debug.LogWarning("Card Options already contains that card!");
                return;
            }
            i++;
        }
        incompleteEncounters.Add(newEnc);
    }

    public static void AddIntroEncounter(Encounter newEnc)
    {
       //First check if the player already has that card.
        int i = 0;
        while (i < incompleteIntroEncs.Count)
        {
            if (incompleteIntroEncs[i] == newEnc)
            {
                Debug.LogWarning("Card Options already contains that card!");
                return;
            }
            i++;
        }
        incompleteIntroEncs.Add(newEnc);
    }

    public static void AddOutroEncounter(Encounter newEnc)
    {
        //First check if the player already has that card.
        int i = 0;
        while (i < incompleteOutroEncs.Count)
        {
            if (incompleteOutroEncs[i] == newEnc)
            {
                Debug.LogWarning("Card Options already contains that card!");
                return;
            }
            i++;
        }
        incompleteOutroEncs.Add(newEnc);
    }

    public static void ClearAllEncounters(){
        foreach (Encounter enc in completedEncounters){completedEncounters.Remove(enc);}
        foreach (Encounter enc in incompleteEncounters) { incompleteEncounters.Remove(enc); }
        foreach (Encounter enc in completedIntroEncs) { completedIntroEncs.Remove(enc); }
        foreach (Encounter enc in incompleteIntroEncs) { incompleteIntroEncs.Remove(enc); }
        foreach (Encounter enc in completedOutroEncs) { completedOutroEncs.Remove(enc); }
        foreach (Encounter enc in incompleteOutroEncs) { incompleteOutroEncs.Remove(enc); }
    }

    #endregion

}
