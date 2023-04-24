using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//this script takescare of loading the levels
public class SceneManaging : MonoBehaviour
{
    public GameObject titlePage;
    public GameObject creditsPage;
    public GameObject resetPage;
    public GameObject soundIcon;
    public GameObject helpPage;
    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void LoadLevel(string name) {
        Cursor.visible = false;
        SceneManager.LoadScene(name);
    }
    public void QuitRequest() {
        Application.Quit();
    }
    public void NextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void CrabLoad() {
        SceneManager.LoadScene("CrabMaze");
    }
    public void TurtleLoad() {
        SceneManager.LoadScene("TurtleLevel");
    }

    public void DolphinLoad(){
        SceneManager.LoadScene("DolphinScene");
    }

    public void Boardwalk(){
        SceneManager.LoadScene("BoardwalkScene");
    }
    public void CloseMenu() {
        
        GameObject.FindGameObjectWithTag("Menu").SetActive(false);
    }
    public void OpenMenu() {
        creditsPage.SetActive(true);
    }
    public void BeginGame() {
        GameObject.FindGameObjectWithTag("Menu").SetActive(false);
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().name == "CrabMaze") {
            Crab.isStarted = true;
        }
        else if (SceneManager.GetActiveScene().name == "TurtleLevel") {
           Turtle.isStarted = true;
        }
        //add one for dolphin
    }

    public void Menu() {
        SceneManager.LoadScene("Menu");
    }
    public void ResetBox() {
        resetPage.SetActive(true);
    }
    public void OpenHelp() {
        helpPage.SetActive(true);
    }
    public void ResetGame() {
        //clears all of the vlaues and reloads the start menu
        PlayData.collectedCrabCards.Clear();
        PlayData.collectedTurtleCards.Clear();
        PlayData.collectedDolphinCards.Clear();
        PlayData.RunStartMenu = true;
        //moves the completed encounter to the incompleted list

        for (int i = 0; i < PlayData.completedEncounters.Count; ++i) {
            PlayData.incompleteEncounters.Add(PlayData.completedEncounters[i]);
        }

        for (int i = 0; i < PlayData.completedIntroEncs.Count; ++i) {
            PlayData.incompleteIntroEncs.Add(PlayData.completedIntroEncs[i]);
        }

        for (int i = 0; i < PlayData.completedOutroEncs.Count; ++i) {
            PlayData.incompleteOutroEncs.Add(PlayData.completedOutroEncs[i]);
        }

        PlayData.completedEncounters.Clear();
        PlayData.completedIntroEncs.Clear();
        PlayData.completedOutroEncs.Clear();
        Menu();
    }
    public void ResetGameWithCredits() {
        //clears all of the vlaues and reloads the start menu
        PlayData.collectedCrabCards.Clear();
        PlayData.collectedTurtleCards.Clear();
        PlayData.collectedDolphinCards.Clear();
        PlayData.runCredits = true;
        
        Menu();
    }
}
