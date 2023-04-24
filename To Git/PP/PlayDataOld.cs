using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayDataOld {
    //Script for holding all of the data between scenes

    //lists all of the values this script holds
    private static bool runStartMenu=true;

    //list for holding the cards
    public static List<string> collectedCrabCards=new List<string>();
    public static List<string> collectedTurtleCards = new List<string>();
    public static List<string> collectedDolphinCards = new List<string>();

    //checks whether to show the startmenu upon loading the menu scene
    public static bool RunStartMenu {
        get {
            return runStartMenu;
        }
        set {
            runStartMenu = value;
        }

    }

    //holds the card values for each minigame
    public static int GetDolphinCardsCount {
        get {
            return collectedDolphinCards.Count ;
        }

    }
    public static int GetTurtleCardsCount {
        get {
            return collectedTurtleCards.Count;
        }
        
    }
    public static int GetCrabCardsCount {
        get {
            return collectedCrabCards.Count;
        }
    }
}
