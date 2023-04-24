using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashlings : MonoBehaviour {
    public GameObject[] waypoints;
    private GameObject currentTarget;
    public int currentIndex=0;
    public float speed;
    public bool firstFlip=true;
    bool isPlaying = true;
	// Use this for initialization
	void Start () {
        currentTarget = waypoints[currentIndex];
        if (firstFlip) {
            transform.localScale = new Vector3(transform.localScale.x*-1 , transform.localScale.y, transform.localScale.z); ;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Vector2.Distance(transform.position,currentTarget.transform.position) < .1f) {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);            //      gameObject.transform.GetComponentInChildren<SpriteRenderer>().flipY = firstFlip;
            //    firstFlip = !firstFlip;

            if (currentIndex+1 == waypoints.Length) {
                currentIndex = 0;
            }
            else {
                currentIndex++;
            }
            currentTarget = waypoints[currentIndex];
            
        }

        //code for making the object patroll the waypoints
        Vector3 nextWaypoint = currentTarget.transform.position - transform.position;
        float angle = Mathf.Atan2(nextWaypoint.y, nextWaypoint.x)*Mathf.Rad2Deg-90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 180); 
        transform.Translate(Vector3.up*Time.deltaTime*speed);

        //disables sound if a screen is up
        if ((Crab.isPaused || Crab.isLost || Crab.isWon)&&isPlaying) {
            gameObject.GetComponent<AudioSource>().Stop();
            isPlaying = false;
        }
        else if(!Crab.isPaused&&!isPlaying){
            gameObject.GetComponent<AudioSource>().Play();
            isPlaying = true;
        }
    }


}
