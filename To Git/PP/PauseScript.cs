using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{

    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name == "CrabMaze") {
            Crab.isPaused = false;
        }
        else if (SceneManager.GetActiveScene().name == "TurtleLevel") {
            Turtle.isPaused = false;
            Turtle.pauseInt = 1;

        }
        else if (SceneManager.GetActiveScene().name == "DolphinScene") {
            DolphinMovement.isPaused = false;
        }
    }
}
