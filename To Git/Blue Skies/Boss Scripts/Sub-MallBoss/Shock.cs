using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script for sending a wave of sparks when the shock attack hits something
public class Shock : MonoBehaviour
{
public bool isParentShock=false;
    public AudioClip shockSound;

    private void OnTriggerEnter2D(Collider2D collision) {
        //when it hits a suface and has been designated as the parent shock..
        if ((collision.tag == "BossGround" || collision.tag == "Wall" ||
            collision.tag == "Bullet") && isParentShock) {
            if (PlayData.playSoundEffects) {
                AudioSource.PlayClipAtPoint(shockSound, GameObject.Find("Main Camera").transform.position,.75f);
            }
            isParentShock = false;
            //send out a wave of shocks and keep them alive for half a second
            for (int i = 0; i < 16; i++) {
                var bolt = Instantiate(gameObject, transform.position, transform.rotation);
                print("Shoot");
                bolt.GetComponent<Collider2D>().enabled = true;
                bolt.GetComponent<Rigidbody2D>().AddForce(10 * transform.up * 100);
                transform.Rotate(0, 0, 360 / 16);
                Destroy(bolt, .45f);
                Destroy(gameObject);
            }
        }
    }
}
