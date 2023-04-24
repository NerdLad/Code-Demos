using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script for controlling the ships for the mall boss
public class Ship : MonoBehaviour
{
    public int speed;
    public float rotateSpeed;
    public Transform center;
    float pastAngle;
    public GameObject bullet;
    public GameObject player;
    public GameObject launcher;
    private AudioSource audioSource;
    public AudioClip destroyedSound;
   // public int rotateDirection = 1;
    // Start is called before the first frame update
    void Start()
    {//sets a random rotation at the begining
        center = GameObject.Find("CenterPoint").transform;
        player = GameObject.FindGameObjectWithTag("Player");

        transform.Rotate(0, 0, Random.Range(0, 360));
        InvokeRepeating("Shoot",Random.Range(1,4), 4f);
         audioSource=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //moves the ship
        transform.Translate(Vector2.left*Time.deltaTime*-speed);


        //gets the angle to the center and converts it to 0-360 to match the values of the gameobject rotation
        float angle=Mathf.Atan2(center.position.y-transform.position.y, center.position.x-transform.position.x)
                    *Mathf.Rad2Deg;
                if (angle<0) {
                    angle +=360;
                }

         //rotates towards the point
        transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(0,0,angle), rotateSpeed * Time.deltaTime);

        if (rotateSpeed < 100) {
            rotateSpeed += 25 * Time.deltaTime;
        }
    }
    void Shoot() {
        //look at player
        Vector3 pos = player.transform.position;
        pos -= launcher.transform.position;
        if (PlayData.playSoundEffects) {            
            audioSource.Play();
        }
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        Quaternion armRotation = Quaternion.AngleAxis(angle, transform.forward);
        launcher.transform.rotation = armRotation;
        //fire
        var projectile = Instantiate(bullet, launcher.transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(6 * launcher.transform.right * 100);
        Destroy(projectile, 3);

    }
    //destroys the ship when hit
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Bullet") {
            MallBoss.activeShips.Remove(gameObject);
            if (PlayData.playSoundEffects) {
                AudioSource.PlayClipAtPoint(destroyedSound, GameObject.Find("Main Camera").transform.position);

            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.tag == "Point") {
            rotateSpeed = 50;
        }
    }
}
