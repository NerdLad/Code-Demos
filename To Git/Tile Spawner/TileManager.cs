using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{   //script to handle spawning and keeping track of tiles

    //variables for the manager
    //gets the square size of one of the tiles
    public float sizeOfTiles;

    //how many tiles to spawn infront of the player and side to side
    public int tilesInfrontOfPlayer = 2;
    public int tilesToTheSideOfPlayer = 2;

    //the seed to keep the tiles random but consistant between players
    public int seed;

    //holds the tile positions currently active
    public List<Vector2> activeTilePositions;

    //holds the different tiles
    public GameObject[] tilePrefabs;

    //gets the size of the tile before anything else
    private void Awake() {
        sizeOfTiles = tilePrefabs[0].GetComponent<Renderer>().bounds.size.x;
    }

    // Start is called before the first frame update
    void Start() {
        activeTilePositions = new List<Vector2>();
    }


    //function to spawn tiles once the player moves to a new square
    public void SpawnTiles(Vector2 previousPos, Vector2 currentPos) {
        int xDirection;
        int yDirection;

        //using the players current and previous position, get the direction the player moved
        if (previousPos.x < currentPos.x) {
            xDirection = 1;
        }
        else if (previousPos.x > currentPos.x) {
            xDirection = -1;
        }
        else {
            xDirection = 0;
        }

        if (previousPos.y < currentPos.y) {
            yDirection = 1;
        }
        else if (previousPos.y > currentPos.y) {
            yDirection = -1;
        }
        else {
            yDirection = 0;
        }

        Vector2 spawnPoint;

        //calculate how many spots away, and how many tiles to make, from the player. This is if the player moved in the x direction
        if (xDirection != 0) {
            spawnPoint.x = currentPos.x + (tilesInfrontOfPlayer * xDirection);
            spawnPoint.y = currentPos.y;
            //for loop to spawn the tiles.  When spawning tiles, grab a specific tile given the coordinates it will be spawned at.
            //also make sure that the tile spawned does not already exit in that spot. Lastly, add the tile to the list
            for (int i = (int)currentPos.y - tilesToTheSideOfPlayer; i < currentPos.y + tilesToTheSideOfPlayer + 1; i++) {
                if (!IsTileInList(new Vector2(spawnPoint.x, i))) {
                    Instantiate(GetTileFromPos((int)spawnPoint.x, i),
                    new Vector3((spawnPoint.x * sizeOfTiles) + (sizeOfTiles / 2), 0, (i * sizeOfTiles) + (sizeOfTiles / 2)), Quaternion.identity);
                    activeTilePositions.Add(new Vector2(spawnPoint.x, i));
                }
            }
        }
        //same as above, but if the player moves up or down
        else {
            spawnPoint.x = currentPos.x;
            spawnPoint.y = currentPos.y + (tilesInfrontOfPlayer * yDirection);

            for (int i = (int)currentPos.x - tilesToTheSideOfPlayer; i < currentPos.x + tilesToTheSideOfPlayer + 1; i++) {
                if (!IsTileInList(new Vector2(i, spawnPoint.y))) {
                    Instantiate(GetTileFromPos(i, (int)spawnPoint.y),
                    new Vector3((i * sizeOfTiles) + (sizeOfTiles / 2), 0, (spawnPoint.y * sizeOfTiles) + (sizeOfTiles / 2)), Quaternion.identity);
                    activeTilePositions.Add(new Vector2(i, spawnPoint.y));
                }
            }
        }
    }
    //function for checking if the tile it wants to spawn is already in the list. If it is, send true
    private bool IsTileInList(Vector2 newTilePos) {
        foreach (Vector2 tile in activeTilePositions) {
            if (tile.x == newTilePos.x && tile.y == newTilePos.y) {
                return true;
            }
        }
        return false;
    }
    //function to spawn a square around the players position. Used at the start to make a base of tiles for the player to start
    public void InitialSpawn(Vector2 startingPos) {
        for (float i = startingPos.x - 1; i <= startingPos.x + 1; i++) {
            for (float j = startingPos.y - 1; j <= startingPos.y + 1; j++) {
                if (!IsTileInList(new Vector2(startingPos.x, startingPos.y))) {
                    Instantiate(GetTileFromPos((int)i, (int)j), new Vector3((i * sizeOfTiles) + (sizeOfTiles / 2), 0, (j * sizeOfTiles) + (sizeOfTiles / 2)), Quaternion.identity);

                }
            }
        }
    }
    //function to get a random tile from the list, while taking into account the X and Y position it will take, and the seed.
    //This ensures that no matter what, for this seed, the same tile will always spawn at the same position.
    public GameObject GetTileFromPos(int posX, int posY) {
        Random.InitState((int)((posX * seed) + posY + seed));             //<--this is one of many math "formulas" that generate the randomness. others can be used if you want.                                                                              //For example, doing ()
        return tilePrefabs[Random.Range(0, tilePrefabs.Length)];          //      for example, putting ((posX * seed) + (posY * seed)) makes diagonal rows of tiles
    }
}
