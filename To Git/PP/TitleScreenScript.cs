using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour {

    public Text dCardCount, tCardCount, cCardCount, Ecount;
    public int totalCollectCards;
    public Image[] cardCounts;
    public GameObject soundIcon;
    public GameObject startImage;
    public GameObject credits;

    private void Start() {
        //checks to see if this is the first time the game is opened and to show the start menu
        if (PlayData.RunStartMenu) {
            PlayData.RunStartMenu = false;
        }
        else {
            CLoseMenu();
        }
        if (PlayData.runCredits) {
            credits.SetActive(true);
            PlayData.runCredits = false;
        }
        SetUpCards();

    }

    public void SetUpCards(){
        //Get and display card information
        dCardCount.text = PlayData.GetDolphinCardsCount + "/4";
        tCardCount.text = PlayData.GetTurtleCardsCount + "/4";
        cCardCount.text = PlayData.GetCrabCardsCount + "/4";
        Ecount.text = "Encounters Left:" + PlayData.incompleteEncounters.Count;

        totalCollectCards = PlayData.GetDolphinCardsCount + PlayData.GetTurtleCardsCount + PlayData.GetCrabCardsCount;
        for (int i = 0; i < totalCollectCards; i++)
        {
            cardCounts[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    public void CLoseMenu() {
        startImage.gameObject.SetActive(false);
    }
    public void CreditsLink(string link) {
        Application.OpenURL(link);
    }
}
