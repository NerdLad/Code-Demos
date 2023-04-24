using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//function for controlling the face of the mall boss
public class Face : MonoBehaviour
{
    //speed
    private int xSpeed = 15;
    private int ySpeed = 15;
    //general
    private Animator faceAnimator;
    public GameObject shock;
    public GameObject launcher;
    public GameObject player;
    //audio
    public AudioSource audioSource;
    public AudioClip[] soundEffects;


    private void Start() {
        faceAnimator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();

    }
    // Update is called once per frame
    void Update() {
        //moves the face
        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y, -20);
    }
    //launches a shock bolt at the player
    void Attack() {    
        Vector3 pos = player.transform.position;
            pos -= launcher.transform.position;
            float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            Quaternion armRotation = Quaternion.AngleAxis(angle, transform.forward);
            launcher.transform.rotation = armRotation;
        //fire if above Y -18 to make it easier  
        if (transform.position.y > -18) {
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[0];
                audioSource.Play();
            }
            //launches and makes the new bolt set as a parent bolt for the bolt spread attack
            var projectile = Instantiate(shock, launcher.transform.position, launcher.transform.rotation);
            projectile.GetComponent<Shock>().isParentShock = true;
            projectile.GetComponent<Rigidbody2D>().AddForce(15 * launcher.transform.right * 100);
        }
    }
    //bounces the face off of the walls
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Xwall") {
            xSpeed *= -1;
        }
        if (collision.tag == "Ywall") {
            ySpeed *= -1;
        }
        //damage from bullet and melee collisions and sends it off to the main boss script
        if (collision.tag == "Bullet") {
            MallBoss.health--;
            StartCoroutine(HurtFlash());
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[1];
                audioSource.Play();
            }
            Destroy(collision.gameObject);
        }
        if (collision.tag == "MeleeHitbox") {
            MallBoss.health-=5;
            StartCoroutine(HurtFlash());
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[1];
                audioSource.Play();
            }
        }

    }
    //flashes the boss when hurt
    public IEnumerator HurtFlash() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, .42f, .42f);
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(.075f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, .42f, .42f);
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    //toggles the face animation when entering and leaving phase one
    //starts and stops attacking 
    public void ToggleFace() {
        print("FaceToggle");
        if (IsInvoking("Attack")) {
            CancelInvoke("Attack");
        }
        else {
            InvokeRepeating("Attack", 3, 2f);
            print("attacking");
        }
        faceAnimator.SetTrigger("faceToggle");    
    }
}
