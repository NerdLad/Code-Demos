using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Simple script to hold the info for each tile

    //Values for how many players are near the tile and its position in the grid
    public int playersNear = 0;
    public Vector2 tilePos;
    public TileManager tileManager;

    // Start is called before the first frame update
    //gets the tile manager and its position
    void Start() {
        tileManager = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>();
        tilePos = new Vector2(Mathf.Floor(transform.position.x / tileManager.sizeOfTiles), Mathf.Floor(transform.position.z / tileManager.sizeOfTiles));

    }

    //Function is called whenever the tile enters or exits a players collider. it is given  +1 when a player enters it and -1 when they leave it. 
    //if it hits 0, it destroys itself and removes itself from the tile list. This is so multiple players can be around a tile and it will only be deleted once everyone is gone
    public void EditPlayer(int editNum) {
        playersNear += editNum;
        if (playersNear <= 0) {
            tileManager.activeTilePositions.Remove(tilePos);
            Destroy(gameObject);
        }
    }
}
