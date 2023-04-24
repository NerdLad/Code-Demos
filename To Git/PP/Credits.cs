using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

    private void Start() {

    }
    //makes the credits scroll
    // Update is called once per frame
    void Update() {
        if (gameObject.transform.localPosition.y < 409) {
            gameObject.transform.Translate(0, 50 * Time.deltaTime, 0);
        }
    }
    public void ResetCredits() {
        transform.localPosition = new Vector3(0, -407, 0);
        transform.parent.gameObject.SetActive(false);
    }
}
