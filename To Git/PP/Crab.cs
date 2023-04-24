using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Crab : MonoBehaviour
{
    //setting up variables
    public float speed = 3;
    public static bool isSpacePressed = false;
    public int health = 3;
    private bool eContact = false;
    public float contactTimer = 2;
    public Sprite[] fishCount;
    public Image fishImage;
    private int fishSaved;
    private int cards = 0;
    public float test = 1;
    private Animator anim;
    public Renderer flash;
    public bool isFlash = false;
    private bool isFlashing = true;
    private bool flashLock = false;
    private float flashTimer = 2;
    public int rotateValue;
    public static bool isPaused = false;
    public static bool isLost = false;
    public static bool isWon = false;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject winScreen;
    private float fishTimer = 1;
    private bool canSnipAgain = true;

    public AudioClip[] audioClips;
    new AudioSource audio;

    public GameObject globalSound;
    public int crabTimer;
    private float levelTimer;
    public Text gameOverText;

    public Text timerText;

    public Image lifeImage;
    public Image cardsImage;

    public Sprite[] lifeSprites;
    public Sprite[] cardSprites;
    public AudioSource music;
    public static bool isStarted = false;

    private bool canSnip = true;

    float turnSpeed = 5;
    // Adding in the animator - Steve
    public Animator crabAnimator;
    public Vector3 prevPos;
    public Vector3 currentPos;

    // Use this for initialization
    void Start() {
        //get the animator and sprite renderer of the crab sprite
        anim = transform.GetChild(0).GetComponent<Animator>();
        //flash = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //set the timescale to pause for the instructions screen and set pause to false
        Time.timeScale = 0;
        isPaused = false;
        isWon = false;
        isLost = false;
        //audio componant of the crab
        audio = GetComponent<AudioSource>();

        isStarted = false;
        //remove cards already collected and change the count to match
        foreach (string card in PlayData.collectedCrabCards) {
            Destroy(GameObject.Find(card));
            cards++;
        }
        cardsImage.sprite = cardSprites[cards];
        //sets the timer for the clock and calls the function for displaying the time
        levelTimer = 1.0f;
        crabTimer++;
        ClockCalculate();
        //stops the music if the sound is toggled off
        if (!PlayData.SoundCheck) {
            music.Stop();
        }
    }

    // Update is called once per frame
    void Update() {
        //START OF STEVE'S CODE: SETTING MOVEMENT FOR THE ANIMATOR
        prevPos = currentPos;
        currentPos = transform.position;
        if (Vector3.Distance(prevPos, currentPos)/Time.deltaTime > 0.5f){
            crabAnimator.SetBool("isMoving", true);
        }
        else
        {
            crabAnimator.SetBool("isMoving", false);
        }

        if (crabAnimator.GetCurrentAnimatorStateInfo(0).IsName("PinchToStand"))
        {
            Debug.Log("InPinchToStandState");
            crabAnimator.SetBool("isPinching", false);
        }

        if (crabAnimator.GetCurrentAnimatorStateInfo(0).IsName("PinchToWalk"))
        {
            Debug.Log("InPinchToWalkState");
            crabAnimator.SetBool("isPinching", false);
        }


        // END OF STEVE'S CODE

        //ticks the clock down each second
        levelTimer -= Time.deltaTime;
        if (levelTimer <= 0) {
            ClockCalculate();
            levelTimer = 1;
        }
        //resets the rotate value
        rotateValue = 0;
        //handles the flashing of the crab and invincibility 
        ////if flashing, keep calling flash untill it is false
        if (isFlash) {
            if (flashLock == false) {
                flashLock = true;
                InvokeRepeating("Flash", 0, .08f);
            }
            flashTimer -= Time.deltaTime;

            //if you are still touching the enemy when the time runs out...
            if (flashTimer <= 0 && eContact) {
                //...take more damage here,reset timer, change the health sprite to match the lives
                health--;
                if (PlayData.SoundCheck) {
                    globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[3]);
                }
                lifeImage.sprite = lifeSprites[health - 1];
                flashTimer = 2;
            }
            //if you are not touching the enemy, turn off flashing and reset veriables for next time
            else if (flashTimer <= 0) {
                CancelInvoke();
                flashLock = false;
                flashTimer = 2;
                flash.enabled = true;
                isFlash = false;
            }
        }
        //Key press inputs: get the key pressed, move in the correct direction and add a number to the rotation value
        if (Input.GetKey(KeyCode.RightArrow)) {
            gameObject.transform.Translate((speed) * Time.deltaTime, 0, 0);
            rotateValue += 3;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            gameObject.transform.Translate(-speed * Time.deltaTime, 0, 0);
            rotateValue += 9;
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            gameObject.transform.Translate(0, speed * Time.deltaTime, 0);
            rotateValue += 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            gameObject.transform.Translate(0, -speed * Time.deltaTime, 0);
            rotateValue += 5;
        }

      /////  crabAnimator.SetBool("isPinching", false);
        //if space is pressed, play the sound if audio is enabled, do the snipping animation
        if (Input.GetKeyDown(KeyCode.Space)) {
            isSpacePressed = true;
            //if (canSnip) {
                Snip();
            //    canSnip = false;
            //}

        }
        //if not pressing space, stop animating
        else {
            isSpacePressed = false;
            anim.SetBool("isSnip", false);
        }
        //pause if esc is pressed
        if (Input.GetKeyDown(KeyCode.Escape)&&isStarted) {
            Pause(isPaused);

        }
        //rotates the sprite depending on the rotatevalue
        switch (rotateValue) {
            //N-0
            case 1:
                CrabRotate(0);
                break;

            //E-90
            case 3:
                CrabRotate(-90);
                break;

            //NE-45
            case 4:
                CrabRotate(-45);
                break;

            //S-180
            case 5:
                CrabRotate(-180);
                break;

            //SE-135
            case 8:
                CrabRotate(-135);
                break;

            //W-275
            case 9:
                CrabRotate(-275);
                break;

            //NW-315
            case 10:
                CrabRotate(-315);
                break;

            //SW-225
            case 14:
                CrabRotate(-225);
                break;

        }
        //game over if health is 0, sent the false message
        if (health == 0) {
            print("Game Over");
            GameOver(false);
        }
        //puts a time restriction on when you can free the fish again, stopping duplicate snips on a single fish
        if (!canSnipAgain) {
            fishTimer -= Time.deltaTime;
            if (fishTimer <= 0) {
                canSnipAgain = true;
                fishTimer = 1;
            }
        }

    }

    //Function added by Steve- this will set the snipping animator 
    public void StopSnipping() //bool to false when the snip is finished
    {
        crabAnimator.SetBool("isPinching", false);
    }

    //toggle the sprite rendering
    private void Flash() {
        isFlashing = !isFlashing;
        flash.enabled = isFlashing;
    }
    //snipping function
    public void Snip() {
        anim.SetBool("isSnip", true);
        crabAnimator.SetBool("isPinching", true); //Adding CG Crab Pinching -Steve

        if (PlayData.SoundCheck) {
            audio.clip = audioClips[1];
            audio.Play();
        }
        CancelInvoke("CanSnip");
        Invoke("CanSnip", .5f);
    }

    public void CanSnip() {
        canSnip = true;
        //crabAnimator.SetBool("isPinching", false);
    }

    public void CrabRotate(int direction) {
        float targetAngle = direction;
        transform.GetChild(0).transform.rotation = Quaternion.Slerp(transform.GetChild(0).transform.rotation, Quaternion.Euler(0, 0, targetAngle), turnSpeed * Time.deltaTime);

    }


    //check trigger collisions
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            //set flashing to true, subtract health and update health image
            eContact = true;
            if (!isFlash)
                health--;
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[3]);
            }
            lifeImage.sprite = lifeSprites[health - 1];
            isFlash = true;
        }
        else if (other.tag == "Card") {
            //play the sound, update card and its image, play the sound, and store the cards name so it can be identified later
            cards++;
            cardsImage.sprite = cardSprites[cards];
            other.gameObject.SetActive(false);
            print("Got Card");
            PlayData.collectedCrabCards.Add(other.name);
            if (PlayData.SoundCheck) {
                globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        //if you are touching an animal, and you can snip and press space, play the sound, destroy thr fish after 
        //a delay, set snipping to false, and update the count
        if (other.gameObject.tag == "Animal") {
            if (isSpacePressed && canSnipAgain) {
                if (PlayData.SoundCheck) {
                    globalSound.GetComponent<AudioSource>().PlayOneShot(audioClips[0]);
                }
                Destroy(other.gameObject, 0.3f);
                canSnipAgain = false;
                fishSaved++;
                fishImage.sprite = fishCount[fishSaved];
            }
            //win the game if all 4 are gotten
            if (fishSaved == 4) {
                Win();
            }
        }
    }
    //when leaving the enemy, reset invincibility values
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            eContact = false;
            contactTimer = 2;
        }
    }

    //if paused, open the screen, set time to 0
    //if not, do the opposite
    private void Pause(bool pauseState) {
        if (!pauseState) {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            isPaused = true;
        }
        if (pauseState) {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            isPaused = false;
        }
    }
    //if game over, set time to 0, turn on game over screen, and display a message depending on what the condition is
    private void GameOver(bool timeRanOut) {
        Time.timeScale = 0;
        isLost = true;
        if (!gameOverScreen.activeSelf) {
            gameOverScreen.SetActive(true);
        }

        if (timeRanOut) {
            gameOverText.text = "You did not save the animals in time!";
        }
        else {
            gameOverText.text = "You ran out of lives!";
        }
    }
    //if won, set the win screen
    private void Win() {
        Time.timeScale = 0;
        winScreen.SetActive(true);
        isWon = true;
        if (PlayData.SoundCheck) {
            music.GetComponent<AudioSource>().Stop();
            music.GetComponent<AudioSource>().PlayOneShot(audioClips[4]);
        }
    }
    //calculate the time to display for the timer and check if its 0 for game over
    public void ClockCalculate() {
        crabTimer--;
        if (crabTimer <= 0) {
            GameOver(true);
        }
        float minuets = Mathf.Floor(crabTimer / 60);
        float seconds = Mathf.RoundToInt(crabTimer % 60);
        //add a leading 0 if seconds is less then 10, and make it 00 if it is 60
        if (seconds < 10) {
            timerText.text = minuets + ":0" + seconds;
        }
        else if (seconds == 60) {
            timerText.text = minuets + ":00";
        }
        else {
            timerText.text = minuets + ":" + seconds;
        }
    }

}