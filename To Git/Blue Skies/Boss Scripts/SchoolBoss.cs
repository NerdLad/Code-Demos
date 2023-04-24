using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//script for controlling the school boss
public class SchoolBoss : MonoBehaviour
{//general variables
    private AudioSource audioSource;
    public AudioSource DamagedSoundSource;
    public AudioClip[] soundEffects;
    public GameObject player;
    public Transform playerSpawn;
    public GameObject face;
    public Animator faceController;
    private SpriteRenderer faceRenderer;
    public GameObject mouthFire;
    private bool isIdle = true;
    private bool attackAftermove = false;
    private bool flashFace = false;
    private float t = 0;
    private int flashDirection = 1;
    //health and hit 
    private int hitsTillAmmoDrop = 8;
    public Slider healthBar;
    public int health = 100;
    public GameObject ammoDrop;
    public GameObject healthDrop;
    public GameObject winMessage;
    public GameObject faceVulnerableSpot;
    //distance for face to move up or down
    public float faceoffset;
    private float faceDirection;

    private float faceY;
    private bool isFaceMoving = false;
    private bool isAttacking = false;

    public Transform launcher;
    public GameObject fireball;
    public GameObject bigFireball;
    private int bigAttackChance = 2;
    private bool attackAfterTrick = false;
    //fog attacks
    private bool isFogging = false;
    private Color fogColor;
    public GameObject mainFog;
    public GameObject[] miniFogs;
    public Transform cloudLauncher;
    public GameObject smog;
    

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        faceRenderer = face.GetComponent<SpriteRenderer>();
        fogColor.r = 1;
        fogColor.g = 1;
        fogColor.b = 1;
    }

    // Update is called once per frame
    void Update() {
        //starting block
        if (!isFaceMoving && !isAttacking && isIdle) {
            print("AtTheStart");
            //decide if it wants to start fogging
            if(Random.Range(0, 10) < 7 && health < 70&&!isFogging) {
                StartCoroutine(FogAttack());
                isFogging = true;
                smog.SetActive(true);
            }
            //chance to move up then down then attack, or just move once and attack(trick attack)
            if (Random.Range(0, 10) <5&&health<50) {
                StartCoroutine(TrickFaceSetUp());
                print("TrickMove");
            }
            else {
                //begin the attack
                Invoke("MoveAndAttack", .5f);
                print("SingleMove");
            }
            isIdle = false;
        }
        //attacks right after trick is done
        if (attackAfterTrick) {
            if (!isFaceMoving) {
                Attack();
                attackAfterTrick = false;
            }
        }
        //move the face when this is on
        if (isFaceMoving) {
            if (attackAftermove) {
                MoveFace(true);
            }
            else {
                MoveFace(false);
            }

        }
        //Win Condition
        if (health <= 0) {
            winMessage.gameObject.SetActive(true);
            PlayData.isSchoolComplete = true;
            Time.timeScale = 0;
            GameObject.Find("Music").GetComponent<AudioSource>().Stop();
            Destroy(gameObject);
        }
        //flash the face when charging the attack
        if (flashFace) {
            if (t > 1) {
                flashDirection= -1;
            }
            else if (t < 0) {
                flashDirection = 1;
            }
            t += 5 * flashDirection* Time.deltaTime;
            faceRenderer.color = Color.Lerp(Color.white, Color.red, t);
        }


    }
    //routine for the trick face
    //decide which direction to move, then how many times to move, then attack
    public IEnumerator TrickFaceSetUp() {
        if (Random.Range(0, 2) == 0) {
            faceDirection = faceoffset;
        }
        else {
            faceDirection = -faceoffset;
        }
        //randomly decide how many times to move the face before attacking
        int attackTime;
        attackTime = Random.Range(2, 5);
        for (int i = attackTime; i > 0; i--) {
            Invoke("TrickFace", i);
        }
        yield return new WaitForSeconds(attackTime+.5f);
        attackAfterTrick = true;
    }

    // moves the face for the trick function
    public void TrickFace() {
        faceDirection = -faceDirection;
        isFaceMoving = true;
    }
    //decide which direction to move, then move the face
    public void MoveAndAttack() {
        isFaceMoving = true;
        attackAftermove = true;
        if (Random.Range(0, 10) <5) {
            faceDirection = faceoffset;

        }
        else {
            faceDirection = -faceoffset;
        }
    }
    //attacking function, decide to do a big or small attack
    public void Attack() {
        if (bigAttackChance < Random.Range(0, 10)&&health<80) {

            StartCoroutine(BigAttack());
            print("BigAttack");
        }
        else {
            StartCoroutine(BasicAttack());
            print("BasicAttack");
        }


    }
    // basic attack, open the mouth, create and launch a fireball, then close the mouth
    public IEnumerator BasicAttack() {
        faceController.SetBool("isFaceOpen", true);
        yield return new WaitForSeconds(0.30f);
        mouthFire.SetActive(true);
        faceVulnerableSpot.SetActive(true);
        yield return new WaitForSeconds(0.35f);

        var projectile = Instantiate(fireball, launcher.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(10 * -launcher.transform.right * 100);
        Destroy(projectile, 3);
        if (PlayData.playSoundEffects) {
            audioSource.clip = soundEffects[1];
            audioSource.Play();
        }
        yield return new WaitForSeconds(.4f);
        faceController.SetBool("isFaceOpen", false);
        yield return new WaitForSeconds(0.167f);
        mouthFire.SetActive(false);
        faceVulnerableSpot.SetActive(false);
        MultiAttackChance();

    }

    //big attack, same as small attack, but launches many fireballs
    public IEnumerator BigAttack() {
        int initialHealth = health;
        faceController.SetBool("isFaceOpen", true);
        if (PlayData.playSoundEffects) {
            audioSource.clip = soundEffects[3];
            audioSource.Play();
        }
        yield return new WaitForSeconds(0.25f);
        mouthFire.SetActive(true);
        faceVulnerableSpot.SetActive(true);
        flashFace = true;
        yield return new WaitForSeconds(1.5f);
        flashFace = false;
        faceRenderer.color = Color.white;
        //if health falls more than 6, end the attack early
        for (int i = 0; i < 10; i++) {
            if (health < 90&&initialHealth-health<6) {
                var projectile = Instantiate(bigFireball, launcher.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().AddForce(20 * -launcher.transform.right * 100);
                Destroy(projectile, 3);
                if (PlayData.playSoundEffects) {
                    audioSource.clip = soundEffects[1];
                    audioSource.Play();
                }
                yield return new WaitForSeconds(.25f);
            }
            
        }

        faceController.SetBool("isFaceOpen", false);
        yield return new WaitForSeconds(0.167f);
        mouthFire.SetActive(false);
        faceVulnerableSpot.SetActive(false);
        //dispense health chance
        if (Random.Range(0, 10) < 2&&health<90) {
            print("dispense health");
            var healthItem=Instantiate(healthDrop, launcher.position, Quaternion.identity);
            //launch the item
            LaunchItem(healthItem);
        }
        yield return new WaitForSeconds(1f);
        MultiAttackChance();
    }
    //launches the health or ammo out for the player when takes 8 damage
    public void LaunchItem(GameObject item) {
        Vector3 dir = Quaternion.AngleAxis(130, Vector3.forward) * Vector3.right;
        item.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(900,990));
    }


    //puts up a fog wall and sends clouds of steam for a time
    public IEnumerator FogAttack() {
        isFogging = true;
        print("Fog Attack");
        if (PlayData.playSoundEffects) {
            AudioSource.PlayClipAtPoint(soundEffects[2], GameObject.Find("Main Camera").transform.position, .6f);
        }
            while (fogColor.a < 1) {
            yield return new WaitForSeconds(0.03f);
            fogColor.a += .01f;
            mainFog.GetComponent<SpriteRenderer>().color=fogColor;
        }
        InvokeRepeating("FogAttackClouds", 0, 1);
        yield return new WaitForSeconds(15);
        CancelInvoke("FogAttackClouds");
        yield return new WaitForSeconds(1);
        //clears the air
        while (fogColor.a >0) {
            yield return new WaitForSeconds(0.03f);
            fogColor.a -= .01f;
            mainFog.GetComponent<SpriteRenderer>().color = fogColor;
        }
        smog.SetActive(false);

        //cooldown
        yield return new WaitForSeconds(15);
        isFogging = false;

    }
    //function for clouds to spawn at a random height
    public void FogAttackClouds() {
        
        cloudLauncher.position = new Vector2(cloudLauncher.position.x,
            Random.Range(50, 86));
        int cloud = Random.Range(0, miniFogs.Length);
        var cloudProjectile = Instantiate(miniFogs[cloud], cloudLauncher.position, Quaternion.identity);
        cloudProjectile.GetComponent<Rigidbody2D>().AddForce(Random.Range(5, 15) * -launcher.transform.right * 100);

        Destroy(cloudProjectile, 15);
    }
    //chance to attack again after attacking, or finish the attack
    public void MultiAttackChance() {
        //based on health
        isFaceMoving = true;
        if (Random.Range(0, 20) < 7&&health<60) {
            faceDirection =-faceDirection;
            print("MultiAttack");
            attackAftermove = true;
        }
        else {
            faceDirection = 0;
            attackAftermove = false;
            isAttacking = false;
            print("finished");
        }
    }

    //move the face up or down depending on the face direction variable, and call attack if specified
    public void MoveFace(bool attackAfter) {

        if (faceDirection < 0f) {
            face.transform.Translate(0, -15 * Time.deltaTime, 0);
            if (face.transform.localPosition.y < faceDirection) {
                isFaceMoving = false;

                if (attackAfter) {
                    isAttacking = true;
                    Attack();
                }
            }
        }
        else if (faceDirection > 0) {
            face.transform.Translate(0, 15 * Time.deltaTime, 0);
            if (face.transform.localPosition.y > faceDirection) {
                isFaceMoving = false;
                isAttacking = true;
                if (attackAfter) {
                    isAttacking = true;
                    Attack();
                }
            }

        }
        //if the direction is 0

        else {

            if (face.transform.localPosition.y < 0f) {
                face.transform.Translate(0, 10 * Time.deltaTime, 0);
                if (face.transform.localPosition.y > faceDirection) {
                    isFaceMoving = false;
                    isIdle = true;
                }
            }
            else if (face.transform.localPosition.y > 0) {
                face.transform.Translate(0, -10 * Time.deltaTime, 0);
                if (face.transform.localPosition.y < faceDirection) {
                    isFaceMoving = false;
                    isIdle = true;
                }
            }
        }
    }
    //damage the boss when hit
    public void TakeDamage(int damageAmount) {
        health -= damageAmount;
        hitsTillAmmoDrop--;
        if (PlayData.playSoundEffects) {
            DamagedSoundSource.clip = soundEffects[0];
            DamagedSoundSource.Play();
        }
        if (hitsTillAmmoDrop <= 0) {
            var ammoItem=Instantiate(ammoDrop, launcher.position, Quaternion.identity);
            //launch the item
            LaunchItem(ammoItem);
            hitsTillAmmoDrop = 5;
        }
        healthBar.value = health;
        StartCoroutine(HurtFlash());
    }
    //flash when hit
    public IEnumerator HurtFlash() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1,.42f,.42f);
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(.075f);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .42f, .42f);
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;


    }
    //collision detection for damage
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Bullet") {
            TakeDamage(2);
        }
        else if(collision.tag == "MeleeHitbox") {
            TakeDamage(5);
            
        }
    }
    //resets the boss when you die
    public void Respawn() {
        health = 100;
        healthBar.value = health;
        Debug.Log("sdf");
        GameObject[] extraItems =GameObject.FindGameObjectsWithTag("Item");
        for(int i = 0; i < extraItems.Length; i++) {
            Destroy(extraItems[i]);
        }
        GameObject[] extraAttacks = GameObject.FindGameObjectsWithTag("FireBall");
        for (int i = 0; i < extraAttacks.Length; i++) {
            Destroy(extraAttacks[i]);
        }
        CancelInvoke("FogAttackClouds");
        smog.SetActive(false);
        player.transform.position = playerSpawn.position;
        mainFog.GetComponent<SpriteRenderer>().color=Color.clear;
        Time.timeScale = 1;
    }
}

