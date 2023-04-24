using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabCamera : MonoBehaviour {
    public GameObject crab;
    public float offset;
	// Use this for initialization
	void Start () {
		//-39,11
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(crab.transform.position.x, crab.transform.position.y, -10);
        if (Vector2.Distance(crab.transform.position, transform.position) > offset) {
            // transform.position = new Vector3(crab.transform.position.x,crab.transform.position.y,-10);
            //transform.position = new Vector3(crab.transform.position.x + offset,transform.position.y, -10);

        }
    }
}
