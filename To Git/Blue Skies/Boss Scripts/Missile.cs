using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script for the missile for the office Boss
public class Missile : MonoBehaviour
{
    public GameObject Explosion;
    public AudioClip explosionSound;
    //creates an explosion at the point of impact and plays the sound
    private void OnCollisionEnter2D(Collision2D collision) {
        if (PlayData.playSoundEffects) {
            AudioSource.PlayClipAtPoint(explosionSound, GameObject.Find("Main Camera").transform.position);
        }
        Destroy(Instantiate(Explosion, transform.position, Quaternion.identity), .25f);
            Destroy(gameObject);
        if (collision.gameObject.name.Contains("David")){
            collision.gameObject.GetComponent<DavidController>().OnPlayerHit(gameObject, 1);
        }
        
    }
}
