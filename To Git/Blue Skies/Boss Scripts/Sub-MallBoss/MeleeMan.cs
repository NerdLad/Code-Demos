using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script for controlling the ninjas for the mall boss
public class MeleeMan : MonoBehaviour
{
    public AudioClip oof;
    public int movementSpeed = 10;
    public int actuialSpeed = 0;
    private bool hasJumped = false;
    private Animator animator;
    private bool runRight = true;
    private LayerMask ground;
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    private SpriteRenderer ninjaSprite;
    private Color initial;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        ground = LayerMask.GetMask("Ground");
        ninjaSprite = gameObject.GetComponent<SpriteRenderer>();
        initial = ninjaSprite.color;
    }

    // Update is called once per frame
    void Update() {
        //ground checking
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        animator.SetBool("isGrounded", isGrounded);

        transform.Translate(actuialSpeed * Time.deltaTime, 0, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        //changes what the ninja will do depending on what it touches
        switch (collision.tag) {
            case "Ground":
                print("grounded");
                if (Random.Range(0, 2) == 0) {
                    actuialSpeed = movementSpeed;
                    runRight = true;
                    GetComponent<SpriteRenderer>().flipX = runRight;
                }
                else {
                    actuialSpeed = -movementSpeed;
                    movementSpeed *= -1;
                    runRight = false;
                    GetComponent<SpriteRenderer>().flipX = runRight;
                }
                break;
            case "Wall":
                if (!hasJumped) {
                    movementSpeed *= -1;
                    actuialSpeed *= -1;
                    print("flipped");
                }
                runRight = !runRight;
                GetComponent<SpriteRenderer>().flipX = runRight;
                break;
            case "Player":
                Destroy(gameObject);
                print("hit player");
                DavidController david = collision.GetComponent<DavidController>();
                if (david.canHit) {

                    david.OnMeleeManHit();
                }

                break;
            case "MeleeHitbox":
                if (PlayData.playSoundEffects) {
                    AudioSource.PlayClipAtPoint(oof, GameObject.Find("Main Camera").transform.position, .75f);
                }
                    actuialSpeed = 0;
                if (!runRight) {
                    Vector3 dir = Quaternion.AngleAxis(50, Vector3.forward) * Vector3.right;
                    gameObject.GetComponent<Rigidbody2D>().AddForce(dir * 1000);
                }
                else {
                    Vector3 dir = Quaternion.AngleAxis(130, Vector3.forward) * Vector3.right;
                    gameObject.GetComponent<Rigidbody2D>().AddForce(dir * 1000);
                }
                StartCoroutine(MeleeFlash());
                foreach (Collider2D c in GetComponents<Collider2D>()) {
                    c.enabled = false;
                }
                Destroy(gameObject, 2);
                break;
            case "LaunchPad":
                //chance to launch in the air
                if (Random.Range(0, 2) == 0) {
                    hasJumped = true;
                    runRight = !runRight;
                    GetComponent<SpriteRenderer>().flipX = runRight;
                    if (actuialSpeed < 0) {// moving left
                        Vector3 dir = Quaternion.AngleAxis(70, Vector3.forward) * Vector3.right;
                        gameObject.GetComponent<Rigidbody2D>().AddForce(dir * (3.2f * 1000));

                    }
                    else { //moving right
                        Vector3 dir = Quaternion.AngleAxis(115, Vector3.forward) * Vector3.right;
                        GetComponent<Rigidbody2D>().AddForce(dir * (3.2f * 1000));
                    }
                    actuialSpeed = 0;
                    Invoke("RestoreSpeed", 2.5f);
                }
                break;
            case "Bullet":
                if (isGrounded) {
                    //incase another bullet comes while already paused
                    CancelInvoke("RestoreSpeed");
                    animator.SetTrigger("catch");
                    actuialSpeed = 0;
                    Invoke("RestoreSpeed", .167f);
                    Destroy(collision.gameObject);
                }
                break;
        }
    }
    //flash when damaged
    public IEnumerator MeleeFlash() {
        for (int i = 0; i < 10; i++) {
            //visible
            yield return new WaitForSeconds(.1f);
            ninjaSprite.color = Color.clear;

            //invisible
            yield return new WaitForSeconds(.1f);
            ninjaSprite.color = initial;

        }
    }
    void RestoreSpeed() {
        actuialSpeed = movementSpeed;
    }

}
