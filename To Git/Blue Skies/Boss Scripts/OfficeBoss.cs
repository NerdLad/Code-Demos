using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//script for controlling the office boss
public class OfficeBoss : MonoBehaviour
{
    //general
    public GameObject player;
    public GameObject ammoDrop;
    public GameObject healthDrop;
    private float healthdropTimer = 45;
    public float hitsTillAmmoDrop = 8;
    private bool hasStarted = false;
    public Transform playerSpawn;

    //bob parameters
    private float defaultBossHight;
    public float bossBob;
    public float bobSpeed;
    private float bossBobSpeed;
    //movement parameters 
    public float moveSpeed;
    private float moveVelocity;
    //swoop
    public float swoopValue = 0;
    private bool isSwooping=false;
    private float swoopHeight;
    public float swoopX;
    public float swoopY;
    private bool isOnScreen = true;
    
    //gun
    public Transform gunPivot;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float force = 10.10f;

    //phases
    public bool isSecondPhase = false;
    private bool dropRoof = false;
    public GameObject explosion;
    public Transform explosionPoint;
    private float explosionTimer=.5f;
    public GameObject phaseTransitionLocation;
    private Vector3 homeLocation;
    public static bool transitionBoss = false;
    public GameObject roof;
    float transitionStartTime;

    //phase2
    public GameObject platform1;
    public GameObject platform2;
    private bool pShoot = false;
    public GameObject platformTarget;
    //health
    public int health = 100;
    public Slider slider;
    public GameObject winMessage;
    //audio
    public AudioClip[] soundEffects;
    public AudioSource audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        //sets up the variables
        defaultBossHight = transform.position.y;
        print(defaultBossHight);
        bossBobSpeed = bobSpeed;
        InvokeRepeating("Fire", 5, 3);
        moveVelocity = moveSpeed;
        slider.maxValue = health;
        //audio
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        //win condition
        if (health < 0) {
            winMessage.SetActive(true);
            PlayData.isOfficeComplete = true;
            Time.timeScale = 0;
            GameObject.Find("Music").GetComponent<AudioSource>().Stop();
            Destroy(gameObject);
        }
        //movement
        if (transform.position.y > defaultBossHight + bossBob) {
            bossBobSpeed = -bobSpeed;
        }
        else if (transform.position.y < defaultBossHight - bossBob) {
            bossBobSpeed = bobSpeed;
        }
        if (!transitionBoss) {
            transform.Translate(moveSpeed * Time.deltaTime, bossBobSpeed * Time.deltaTime, 0);
        }

        //chance to swoop if boss is at the right or left sides
        if (transform.position.x > -7 && !isSwooping) {
            FlipAndSwoopChance(false);
        }

        else if (transform.position.x < -89 && !isSwooping) {
            FlipAndSwoopChance(true);
        }
        else {
            isOnScreen = true;
        }
        //slowly drops the roof and instantiates explosions around every half second
        if (dropRoof&&roof.activeSelf) {
            if (explosionTimer <= 0) {
                explosionPoint.transform.localPosition = new Vector3(Random.Range(-8, 9), explosionPoint.transform.localPosition.y, 0);
               Destroy(Instantiate(explosion, explosionPoint.position, Quaternion.identity),.5f);
                explosionTimer = .15f;
            }
            else {
                explosionTimer -= Time.deltaTime;
            }
            roof.transform.Translate(0, -2.75f * Time.deltaTime, 0);
        }
        //swoop the boss if this is set to true
        if (isSwooping) {
            Swoop();
        }
        //health drop timer and spawn in a random spawner
        if (healthdropTimer <= 0) {
            Transform droplocation=gameObject.transform;
            switch (Random.Range(0, 3)) {
                case 0:droplocation = platform1.transform;
                    break;
                case 1:droplocation = platform2.transform;
                    break;
                case 2: droplocation = phaseTransitionLocation.transform;
                    break;
            }
            Instantiate(healthDrop, droplocation.position, Quaternion.identity);
            healthdropTimer = 30;
        }
        else {
            healthdropTimer -= Time.deltaTime;
        }

        //chance to platform shoot if boss is in the middle and is second phase
        if (transform.position.x > -44.5 && transform.position.x < -44f &&
            Random.Range(0, 10) < 1 && isSecondPhase && !isSwooping && !pShoot) {
            print("pshooting");
            pShoot = true;
            
        }

        //switch for the platform shoot
        if (pShoot) {
            PlatformShoot();
        }

        //gun rotate towards Player

        Vector3 pos = player.transform.position;
        pos -= gunPivot.transform.position;
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        Quaternion gunRotation = Quaternion.AngleAxis(angle, transform.forward);
        if (!transitionBoss) {
            gunPivot.transform.rotation = gunRotation;
        }
        //halth amount needed to move to the next phase
        if (health < 75 && !isSecondPhase && !transitionBoss) {
            if (ChaChaRealSmooth(phaseTransitionLocation,2.5f)) {
                StartCoroutine(PhaseTransition());
            }
        }

    }
    //flips the boss and has the chance to swoop down
    void FlipAndSwoopChance(bool right) {
        if (right) {
            transform.localScale = new Vector3(-1, 1, 1);
            moveSpeed = -moveVelocity;
            if (Random.Range(0, 4) <2 && isOnScreen&&hasStarted) {
                isSwooping = true;
            }
        }
        else {
            transform.localScale = new Vector3(1, 1, 1);
            moveSpeed = moveVelocity;

            if (Random.Range(0, 4) < 2 && isOnScreen && hasStarted) {
                isSwooping = true;
            }

        }
    }
    //smoothly moves the boss to the specified location
    //returns false if the movement is not done, true if it is, and takes a gameobject as the target locaion to move to
    bool ChaChaRealSmooth(GameObject targetObject,float speed) {
        Vector3 targetLocation = targetObject.transform.position;
        if (Vector3.Distance(transform.position, targetLocation) > 1) {
            if (homeLocation == Vector3.zero) {
                transitionStartTime = Time.time;
                homeLocation = transform.position;
            }
            float t = (Time.time - transitionStartTime) / speed;
            transform.position = new Vector3(
                Mathf.SmoothStep(homeLocation.x, targetLocation.x, t),
                Mathf.SmoothStep(homeLocation.y, targetLocation.y, t), 0);
            return false;
        }
        else {
            homeLocation = Vector3.zero;

            return true;
        }
    }

    //sequence for the transition from first to second phase, moving to the middle an shooting the roof 
    public IEnumerator PhaseTransition() {
        transitionBoss = true;


        //look at the platforms
        Vector3 pos = roof.transform.position;
        pos -= gunPivot.transform.position;
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        Quaternion armRotation = Quaternion.AngleAxis(angle, transform.forward);
        gunPivot.transform.rotation = armRotation;
        
        //fire the shots at the platform
        yield return new WaitForSeconds(1);
        for(int i = 0; i<3; i++) {
            Fire();
            yield return new WaitForSeconds(.3f);
        }
        //drop the roof
        if (health < 80) {
            dropRoof = true;
        if (PlayData.playSoundEffects) {
            audioSource.clip = soundEffects[1];
            audioSource.Play();
        }
        yield return new WaitForSeconds(1f);
        
        // resume phase 2
        transitionBoss = false;
        isSecondPhase = true;
        yield return new WaitForSeconds(9f);
            if (health < 80) {
                roof.SetActive(false);
            }
        }
    }
    //shoot at the player
    void Fire() {
        
        if (!hasStarted) {
            hasStarted = true;
        }
        if (!isSwooping) {
            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            //sets the velocity of the bullet
            bullet.GetComponent<Rigidbody2D>().AddForce(force * gunPivot.transform.right * 100);
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[0];
                audioSource.Play();
            }

            // Destroy the bullet after 3.5 seconds
            Destroy(bullet, 3.5f);
        }
    }
    //swoop by moving quickly and bobbing down and up through Msthf.sin
    void Swoop() {
        if (swoopValue == 0) {
            swoopHeight = transform.position.y;
            float swoopXDir = swoopX;

            if (transform.position.x < -40) {
                swoopXDir = -swoopX;
            }
            moveSpeed = swoopXDir;

        }
        transform.position=new Vector3(transform.position.x,swoopHeight+(swoopY*Mathf.Sin(swoopValue*Mathf.Deg2Rad)));
        if (swoopValue > -180) {
            swoopValue -= Time.deltaTime * 50;

        }
        else {
            
            isSwooping = false;
            swoopValue = 0;
            isOnScreen = false;
            //bossBobSpeed = bossBob;
        }
        
    }
    //shoots a spread of missiles at the platform
    //randomly picks the target, moves to it, and starts the routine to fire
    void PlatformShoot() {
        transitionBoss = true;
        if (platformTarget==null) {
            if (Random.Range(0, 2) == 0) {
                platformTarget = platform2;
            }
            else {
                platformTarget = platform1;
            }
        }
        
        if (ChaChaRealSmooth(platformTarget,1.3f)) {
            platformTarget = null;
            pShoot = false;
            StartCoroutine(MultiShot());
        }
    }
    //fire missiles around a rotation
    public IEnumerator MultiShot() {
        //CancelInvoke("Fire");
         gunPivot.rotation = new Quaternion(0, 0, -150, 0);
         gunPivot.Rotate(0, 0, 40);
        if (transform.localScale.x < 0) {
            gunPivot.Rotate(0, 0, 180);
        }

        for (int i=0; i < 20; i ++){
            if (health < 80) {
                gunPivot.Rotate(0, 0, 5);
                Fire();
                yield return new WaitForSeconds(.1f);
            }
        }

        transitionBoss = false;
    }
    //collision checking for damage
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Bullet") {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "MeleeHitbox") {
            TakeDamage(5);
        }
    }
    public void TakeDamage(int damageAmount) {
        health -= damageAmount;
        hitsTillAmmoDrop--;
        if (hitsTillAmmoDrop <= 0) {
            Instantiate(ammoDrop, transform.position, Quaternion.identity);
            hitsTillAmmoDrop = 8;
        }
        slider.value = health;
        StartCoroutine(HurtFlash());
    }
    //flashes the boss when damaged
    public IEnumerator HurtFlash() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, .42f, .42f);
        yield return new WaitForSeconds(.02f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, .42f, .42f);
        yield return new WaitForSeconds(.02f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    //resets the boss when you die
    public void Respawn() {
        health = 120;
        slider.value = health;

        GameObject[] extraItems = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < extraItems.Length; i++) {
            Destroy(extraItems[i]);
        }
        GameObject[] extraAttacks = GameObject.FindGameObjectsWithTag("Missile");
        for (int i = 0; i < extraAttacks.Length; i++) {
            Destroy(extraAttacks[i]);
        }
        CancelInvoke("Fire");
        isSwooping = false;
        hasStarted = false;
        isSecondPhase = false;
        transitionBoss = false;
        dropRoof = false;
        pShoot = false;

        roof.SetActive(true);
        roof.transform.position = new Vector3(-44, 4.5f, -7.98f);
        transform.localScale = new Vector3(1, 1, 1);
        moveSpeed = moveVelocity;
        InvokeRepeating("Fire", 5, 3);
        transform.position = new Vector3(36.5f, 55.75f, 0);
        player.transform.position = playerSpawn.position;
        Time.timeScale = 1;
    }
}
