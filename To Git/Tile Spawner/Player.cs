using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*player script. 
     * 
     * IMPORTANT! IF USING YOUR OWN PLAYER SCRIPT, YOU WILL NEED TO COPY EVERYTHING EXCEPT WHAT IS IN THE FIXED UPDATE
     * If you want, you can use this script for your own main player
     */



    public float walkspeed = 3;
    public bool canMove = true;

    //************** Copy these too
    //values to hold the players pos, last pos if they moved to a new tile, and the tile size
    public Vector2 absolutePos;
    public Vector2 prevPos;
    public float sizeOfTiles;
    public TileManager tileManager;

    // Start is called before the first frame update
    //gets the tile size,  calculate its position in the tile grid from its transform position, and calls the function to spawn its starting tiles
    void Start() {
        sizeOfTiles = tileManager.sizeOfTiles;
        absolutePos = new Vector2(Mathf.Floor(transform.position.x / sizeOfTiles), Mathf.Floor(transform.position.z / sizeOfTiles));
        prevPos = absolutePos;
        tileManager.InitialSpawn(absolutePos);

    }

    // Update is called once per frame
    // Updates its position and sees if its different from its last position, If so, spawn new tiles.
    void Update() {
        absolutePos = new Vector2(Mathf.Floor(transform.position.x / sizeOfTiles), Mathf.Floor(transform.position.z / sizeOfTiles));

        if (absolutePos != prevPos) {
            tileManager.SpawnTiles(prevPos, absolutePos);
        }
        prevPos = absolutePos;
    }
    //No need to copy this to your player. THis just moves it with WASD
    private void FixedUpdate() {
        if (canMove) {
            if (Input.GetKey(KeyCode.A)) {
                transform.Translate(-walkspeed * Time.deltaTime, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D)) {
                transform.Translate(walkspeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(KeyCode.W)) {
                transform.Translate(0, 0, walkspeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S)) {
                transform.Translate(0, 0, -walkspeed * Time.deltaTime);
            }
        }
    }
    // when entering an existing tile, or one that just spawned, add a 1 to its player count.
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Tile")) {
            print("Entered");
            other.GetComponent<Tile>().EditPlayer(1);
        }
    }
    //when exiting a tile, subtract 1 from it
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Tile")) {
            print("exited");
            other.GetComponent<Tile>().EditPlayer(-1);
        }
    }

}

