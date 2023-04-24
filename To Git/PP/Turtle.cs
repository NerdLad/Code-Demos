using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Turtle : MonoBehaviour {
    public float vSpeed;
    public float minMSpeed;
    private float movementSpeed;
    public float boost;
    public float decayRate;

    private int jellies = 0;
    private int life=4;
    private int cards = 0;

  //  public Text jellyText;
    public Image lifeImage;
    public Image hungerImage;
    public Image cardsImage;

    public Sprite[] lifeSprites;
    public Sprite[] hungerSprites;
    public Sprite[] cardSprites;
    public float hungerTimer;
    private float timer;
    float invincTimer = 3;
    public bool canSubtractLife = true;
    public Slider hunger;

    public static bool isPaused = false;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public Text gameOverMessage;
    public GameObject winScreen;
    public static int pauseInt=1;

    public float jellyValue;
    public static bool isStarted = false;

    public AudioSource music;
    public GameObject globalSound;

    public AudioClip[] audioClips;
    new AudioSource audio;
    private bool isWon=false;
    // Use this for initialization
    void Start () {
        hunger.value = .30f;
        timer = hungerTimer;
        Time.timeScale = 0;
        isPaused = false;
        movementSpeed = minMSpeed;
        //remove cards already collected and change the count to match
        foreach (string card in PlayData.collectedTurtleCards) {
            Destroy(GameObject.Find(card));
            cards++;
        }
        cardsImage.sprite = cardSprites[cards];
        CheckHunger();
        isStarted = false;
    }

    // Update is called once per frame
    void Update () {
        //game over check
        if (hunger.value < .12f) {
            GameOver(2);
            print("low hunger");
        }
        else if (hunger.value ==1) {
            Win();
        }
        //trash collision check
        if (!canSubtractLife) {
            invincTimer -= Time.deltaTime;
            if (invincTimer < 0) {
                invincTimer = 3;
                canSubtractLife = true;
            }
        }
        //timer for subtracting a hunger unit every X time
        timer -= Time.deltaTime;
        if (timer < 0) {
            timer = hungerTimer;
            hunger.value -= .01f;
            CheckHunger();
        }

        //clamp the movement speed 
        movementSpeed=Mathf.Clamp(movementSpeed, minMSpeed, minMSpeed+boost);
        if (movementSpeed>minMSpeed) {
            movementSpeed -= decayRate * Time.deltaTime;
        }
        //move the turtle up and down depending on arrow presses
        if (Input.GetKey(KeyCode.UpArrow) && transform.localPosition.y < 3.81f) {
            gameObject.transform.Translate(0, vSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow)&&transform.localPosition.y>-4.9f) {
            gameObject.transform.Translate(0, -vSpeed * Time.deltaTime, 0);
        }
        //move the turtle to the right by moving its parent
        gameObject.transform.parent.Translate(movementSpeed * Time.deltaTime, 0, 0);

        //if the turtle falls behind, move it forward slowly to have it back
        if (transform.localPosition.x < 0&&Time.timeScale==1) {
            transform.Translate((Mathf.Clamp((Vector2.Distance(transform.parent.position, transform.position))/700,0,.01f))*pauseInt, 0, 0);

          //  print(Mathf.Clamp((Vector2.Distance(transform.parent.position, transform.position)) / 500, 0, .01f));
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isStarted) {
            Pause(isPaused);
        }
        if (life == 0) {
            GameOver(1);
            print("low life");
        }


    }
    //collision detections
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Lose") {
           GameOver(1);
        }
        else if(other.tag == "Enemy") {
            life--;
            lifeImage.sprite = lifeSprites[life-1];
            other.gameObject.SetActive(false);
            hunger.value -= .20f;
            CheckHunger();
            movementSpeed = minMSpeed;
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }
        }
        else if (other.tag == "Jelly") {
            jellies++;
            other.gameObject.SetActive(false);
            hunger.value += jellyValue;
            CheckHunger();
            movementSpeed += boost;
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[1]);
            }
        }
        else if (other.tag == "Card") {
            cards++;
            cardsImage.sprite = cardSprites[cards];
            Destroy(other.gameObject);
            PlayData.collectedTurtleCards.Add(other.name);
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[3]);
            }
        }

        else if (other.tag == "Teleport") {
            Win();
        }
        
    }

    //collision with trash
    private void OnCollisionEnter2D(Collision2D collision) {
        if (canSubtractLife) {
            hunger.value -= .05f;
            CheckHunger();
            life--;
            canSubtractLife = false;
            lifeImage.sprite = lifeSprites[life - 1];
            movementSpeed = minMSpeed;
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[5]);
            }
        }   
    }

    //function for checking the hunger and changing the fill image appropriately.
    public void CheckHunger() {
        if (hunger.value <= .25f) {
            hungerImage.sprite = hungerSprites[0];
        }
        else if (hunger.value > .26f && hunger.value <= .60f) {
            hungerImage.sprite = hungerSprites[1];
        }
        else if(hunger.value > .61f) {
            hungerImage.sprite = hungerSprites[2];

        }
    }
    private void Pause(bool pauseState) {
        if (!pauseState) {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            isPaused = true;
            pauseInt = 0;
        }
        if (pauseState) {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            isPaused = false;
            pauseInt = 1;
        }
    }
    //activates the gameover screen with message: 1-ran out of life, 2-Ran out of health
    private void GameOver(int message) {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
        if (message == 1) {
            gameOverMessage.text = ("You ran out of life by eating too many bags or hitting trash piles!");
        }
        else {
            gameOverMessage.text = ("Your hunger meter was depleted!");
        }
    }
    private void Win() {
        if (!isWon) {
            Time.timeScale = 0;
            winScreen.SetActive(true);
            isWon = true;
            if (PlayData.SoundCheck) {
                music.GetComponent<AudioSource>().Stop();
                music.GetComponent<AudioSource>().PlayOneShot(audioClips[4]);
            }
        }
    }
}
