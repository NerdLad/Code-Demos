using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//script to control the mall boss and phases
public class MallBoss : MonoBehaviour
{
    //general variables
    public int startPhase;
    public GameObject character;
    public Slider healthBar;
    public static int health = 100;
    public GameObject winMessage;
    public GameObject ammoDrop;
    public GameObject healthDrop;
    //audio
    public AudioClip[] soundEffects;
    private AudioSource audioSource;
    public AudioSource auxAudioSource;

    //screen 
    public Vector3 bigScale = new Vector3(743, 512, 512);
    public Vector3 smallScale = new Vector3(360, 248, 248);
    bool isScreenDone = true;
    public Face face;

    //phase and control 
    public bool hasStarted = false;
    bool isPhaseTransitionDone = true;
    private int previousPhase;
    private int phase = 0;
    public GameObject[] phaseThings;
    private GameObject activePhase;
    private int completedActionPhases = -1;
    private float idleTimer = 10;
    public List<int> firstPhases = new List<int> { 2, 3, 4 };
    bool canShoot = true;


    //shooting 
    public GameObject ship;
    public static List<GameObject> activeShips = new List<GameObject>();
    public Transform[] spawnPoints;
    public int shipsLeft;
    public GameObject spaceGun;
    public GameObject asteroid;
    public Texture2D curserTexture;

    //melee 
    public GameObject meleeMan;
    public Transform meleeSpawn;

    //parkour 
    public GameObject building;
    public Transform buildingSpawn;
    public GameObject buildingsMove;
    public GameObject goal;
    public GameObject firstBuilding;
    public GameObject lava;
    public Collider2D wallCollider;
    IEnumerator co;
    IEnumerable melee;
    public GameObject fireLauncher;
    public Transform fireLauncherSpawner;


    public float bspeed;
    public float srate;

    // Start is called before the first frame update
    void Start() {
        //sets health
        health = 100;
        co = CreateBuildings();
        //mixes up the order of three phases so all three 
        //can be played before going to full randomness
        for (int i = 0; i < firstPhases.Count; i++) {
            int temp = firstPhases[i];
            int randomIndex = Random.Range(i, firstPhases.Count);
            firstPhases[i] = firstPhases[randomIndex];
            firstPhases[randomIndex] = temp;
        }
        //audio
        audioSource = GetComponent<AudioSource>();

        activePhase = phaseThings[0];
        LoadPhase(startPhase);
    }

    // Update is called once per frame
    void Update() {
        //win condition, move to phase 0 
        healthBar.value = health;
        if (health <= 0&&phase!=0) {
            LoadPhase(0);
        }
        //check if player has 0 health, then remove any stray bullets
        if (character.GetComponent<DavidController>().hitPoints <= 0) {
            ClearBullets();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }


        ////runs at the beginning of a phase, sets up.
        //0=nothing/defeated
        //1=small screen
        //2=shooting
        //3-melee
        //4-parkour
        
        //runs the code for the active phase once and stops the static noise
        if (isScreenDone && !isPhaseTransitionDone) {
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[0];
                audioSource.Stop();
            }
            // sets the active phase to true
            activePhase.SetActive(true);
            switch (phase) {
                case 0:
                    winMessage.SetActive(true);
                    activePhase.SetActive(false);
                    PlayData.ismallComplete = true;
                    //turn off the music, freezes the game and sets the win message
                    GameObject.Find("Music").GetComponent<AudioSource>().Stop();
                    CancelInvoke("EndIdlePhase");
                    DavidController.allowShoot = false;                  
                    Time.timeScale = 0;
                    isPhaseTransitionDone = true;                    
                    break;
                case 1://incriments the completed phases, sets the timer
                    //   for the next phase and toggles the face animation
                    completedActionPhases++;
                    face.Invoke("ToggleFace", idleTimer - .6f);
                    Invoke("EndIdlePhase", idleTimer);

                    isPhaseTransitionDone = true;
                    break;
                case 2://spawns the ships
                    for (int i = 0; i < 3; i++) {
                        var newShip = Instantiate(ship, spawnPoints[i].position, Quaternion.identity);
                        activeShips.Add(newShip);
                        spaceGun.SetActive(true);
                        DavidController.allowShoot = false;
                        Cursor.SetCursor(curserTexture,
                            new Vector2(curserTexture.width * 0.5f, curserTexture.height * 0.5f), CursorMode.ForceSoftware);
                    }
                    if (PlayData.playSoundEffects) {
                        auxAudioSource.clip = soundEffects[3];
                        auxAudioSource.loop = true;
                        auxAudioSource.Play();
                    }
                    shipsLeft = 3;
                    isPhaseTransitionDone = true;
                    break;
                case 3: //starts the routine to spawn the melee men
                    character.transform.position = new Vector3(3, character.transform.position.y, character.transform.position.z);
                    co = SendMeleeEnemies();
                    StartCoroutine(co);
                    if (PlayData.playSoundEffects) {
                        audioSource.clip = soundEffects[4];
                        audioSource.Play();
                    }
                    isPhaseTransitionDone = true;
                    break;
                case 4://moves up the first building untill it reaches the right height
                    firstBuilding.transform.Translate(0, 3 * Time.deltaTime, 0);
                    if (firstBuilding.transform.localPosition.y > -35) {
                        //start moving and making buildings and turns on lava
                        if (PlayData.playSoundEffects) {
                            audioSource.clip = soundEffects[5];
                            audioSource.Play();
                        }
                        co = CreateBuildings();
                        StartCoroutine(co);
                        isPhaseTransitionDone = true;
                        wallCollider.enabled = false;
                        lava.SetActive(true);
                    }
                    break;

            }

        }
        //lets the screen scale if it is not already finished
        if (!isScreenDone) {
            if (phase == 1) {
                MakeScreenBig(smallScale);
            }
            else {

                MakeScreenBig(bigScale);
            }
        }

        //waiting for win condition to move on to the next phase, as well as running each frame
        if (isScreenDone && isPhaseTransitionDone) {
            switch (phase) {
                case 0:
                    break;
                case 1:
                    break;
                case 2://checks if you cleared all of the ships, and moves on if you did
                    //if you can shoot the asteroid gun
                    if (Input.GetMouseButtonDown(0)) {
                        Shoot();
                    }

                    if (activeShips.Count == 0) {
                        spaceGun.SetActive(false);
                        ClearBullets();
                        DavidController.allowShoot = true;
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        //drops ammo
                        Instantiate(ammoDrop, spawnPoints[2].transform.position, Quaternion.identity);
                        auxAudioSource.Stop();
                        auxAudioSource.loop = false;
                        if (PlayData.playSoundEffects) {
                            auxAudioSource.clip = soundEffects[1];
                            auxAudioSource.Play();
                        }
                        LoadPhase(1);
                    }
                    break;
                case 3:
                    //if (Input.GetKeyDown(KeyCode.F)) {
                    //    Instantiate(meleeMan, meleeSpawn.position, Quaternion.identity);
                    //}
                    break;
                case 4:
                    //moves the buildings
                    buildingsMove.transform.Translate(-bspeed * Time.deltaTime, 0, 0);
                    break;
            }

        }
    }
    //function for creatings the buildings and fire launchers
    public IEnumerator CreateBuildings() {//-82,  -56, jump up distence: 12.5

        //removes any items that may be on the floor
        GameObject[] extraItems = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < extraItems.Length; i++) {
            Destroy(extraItems[i]);
        }
        //spawns 5 buildings and 5 fire launchers
        for (int i = 0; i < 5; i++) {
            //calculates a random building height based on the height of the last building
            float buildingHeightCalc = Random.Range(-82.0f, Mathf.Clamp(buildingSpawn.position.y + 12.5f, -82, -58));

            buildingSpawn.position = new Vector3(buildingSpawn.position.x, buildingHeightCalc, buildingSpawn.position.z);

            var buildingPlatform = Instantiate(building, buildingSpawn.position, Quaternion.Euler(0, 0, 180));
            buildingPlatform.transform.parent = buildingsMove.gameObject.transform;
            Destroy(buildingPlatform, 10);
            srate += .05f;
            yield return new WaitForSeconds((srate / 2.2f) + .5f);

            //spawn fire sppawners
            Instantiate(fireLauncher, fireLauncherSpawner.position, Quaternion.identity).transform.parent = buildingsMove.gameObject.transform; ;
            yield return new WaitForSeconds(srate / 2.2f);
        }
        //-29 local
        //spawn the goal
        var exit=Instantiate(goal, new Vector3(buildingSpawn.position.x, -29, buildingSpawn.position.z),
            Quaternion.identity);
        exit.transform.parent = buildingsMove.gameObject.transform;
        Destroy(exit, 10);
        srate = 3.5f;
    }


    //function for ending phase 3, called from David, resets the playfield
    public void EndPhase3(bool fellInLava) {
        // lava.transform.position = new Vector3(lava.transform.position.x, -51.5f, lava.transform.position.z);
        //penalty for hittin the lava
        if (fellInLava && character.GetComponent<DavidController>().hitPoints > 1) {

            idleTimer = 4;
            if (PlayData.playSoundEffects) {
                auxAudioSource.clip = soundEffects[2];
                auxAudioSource.Play();
            }
            if (character.transform.position.x < -33) {
                character.transform.position = new Vector3(-29.5f, -29.5f, character.transform.position.z);
            }
            //stop the building spawn and reset the corutine
            character.GetComponent<DavidController>().OnPlayerHit(gameObject, 1);
        }
        else {
            //spawn health and ammo
            Instantiate(healthDrop, spawnPoints[0].transform.position, Quaternion.identity).
            GetComponent<HealthPickup>().healAmount = 2;
            Instantiate(ammoDrop, spawnPoints[1].transform.position, Quaternion.identity);
            if (PlayData.playSoundEffects) {
                auxAudioSource.clip = soundEffects[1];
                auxAudioSource.Play();
            }
        }
        //resets for next run
        StopCoroutine(co);
        activePhase.transform.position = new Vector3(0, 0, -41);
        wallCollider.enabled = true;
        lava.SetActive(false);
        firstBuilding.transform.Translate(0, -10, 0);
        buildingSpawn.transform.position = new Vector3(65.7f, -93.8f, -18);
        buildingsMove.transform.position = Vector2.zero;
        LoadPhase(1);


    }
    //for shooting the space gun
    void Shoot() {
        //look at the mouse point
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        pos -= spaceGun.transform.position;
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;

        Quaternion armRotation = Quaternion.AngleAxis(angle, transform.forward);
        spaceGun.transform.rotation = armRotation;
        //fire
        if (canShoot) {
            if (PlayData.playSoundEffects) {
                audioSource.clip = soundEffects[6];
                audioSource.Play();
            }
            var projectile = Instantiate(asteroid, spaceGun.transform.position, spaceGun.transform.rotation);
            projectile.transform.Rotate(0, 0, 90);
            projectile.GetComponent<Rigidbody2D>().AddForce(8 * spaceGun.transform.right * 100);
            Destroy(projectile, 5);
            canShoot = false;
            Invoke("RestoreShoot", .7f);
        }

    }
    public void RestoreShoot() {
        canShoot = true;
    }
    //ends the idle phase and selects the next phase to play
    public void EndIdlePhase() {
        idleTimer = 10;
        hasStarted = true;
        if (completedActionPhases < 3) {
            //makes sure all three phases are randomly chosen and played before going full random
            previousPhase = firstPhases[completedActionPhases];
            LoadPhase(firstPhases[completedActionPhases]);

        }
        else {
            //loads a random phase while making sure not to do the same phase twice in a row
            int newPhase = Random.Range(2, 5);
            while (newPhase == previousPhase) {
                newPhase = Random.Range(2, 5);
                print("write something");
            }
            previousPhase = newPhase;
            LoadPhase(newPhase);
        }
    }
    //function to spawn the meleemen for phase 3
    public IEnumerator SendMeleeEnemies() {
        //sends 20 melee men
        for (int i = 0; i < 20; i++) {
                 Instantiate(meleeMan, meleeSpawn.position, Quaternion.identity);
                 yield return new WaitForSeconds(.4f);
        }
        yield return new WaitForSeconds(10);
       //drops health and loads the attack phase
            LoadPhase(1);
            Instantiate(healthDrop, spawnPoints[2].transform.position, Quaternion.identity).
                GetComponent<HealthPickup>().healAmount = 2;
            if (PlayData.playSoundEffects) {
                auxAudioSource.clip = soundEffects[1];
                auxAudioSource.Play();
            }
            DavidController.canMove = true;
       
    }
    //scales the screen to the vector size requested
    public void MakeScreenBig(Vector3 size) {

        transform.localScale = Vector3.Lerp(transform.localScale, size, 2 * Time.deltaTime);
        if ((size == bigScale && transform.localScale.x > size.x - .1f) || size == smallScale && transform.localScale.x < size.x + .1f) {
            isScreenDone = true;
        }
    }
    //clears all of the bullets on the game
    void ClearBullets() {
        GameObject[] bulletItems = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bulletItems.Length; i++) {
            Destroy(bulletItems[i]);
        }
    }
    //loads the phase by seting it as active and making the transition done =false and begining the screen scale
    void LoadPhase(int newPhase) {
        //0=nothing
        //1=small screen
        //2=shooting
        //3-melee
        //4-parkour
        print("Starting phase " + newPhase);
        isPhaseTransitionDone = false;
        activePhase.SetActive(false);
        phase = newPhase;
        if (newPhase == 0) {
            return;
        }
        //plays static sound
        if (PlayData.playSoundEffects) {
            audioSource.clip = soundEffects[0];
            audioSource.Play();
        }
        isScreenDone = false;
        switch (newPhase) {
            case 1:
                activePhase = phaseThings[0];
                face.ToggleFace();
                break;
            case 2:
                activePhase = phaseThings[1];
                break;
            case 3:
                activePhase = phaseThings[2];
                break;
            case 4:
                activePhase = phaseThings[3];
                break;

        }
    }

    //resets the boss and values when you die
    public void Respawn() {
        health = 100;
        healthBar.value = health;
        hasStarted = false;
        GameObject[] extraItems = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < extraItems.Length; i++) {
            Destroy(extraItems[i]);
        }

        GameObject[] extraAttacks = GameObject.FindGameObjectsWithTag("Shock");
        for (int i = 0; i < extraAttacks.Length; i++) {
            Destroy(extraAttacks[i]);
        }
        GameObject[] extraAttacks2 = GameObject.FindGameObjectsWithTag("MeleeMan");
        for (int i = 0; i < extraAttacks2.Length; i++) {
            Destroy(extraAttacks2[i]);
        }
        ClearBullets();
        activeShips.Clear();

        face.CancelInvoke("ToggleFace");
        CancelInvoke("EndIdlePhase");
        if (phase == 1) {
            face.ToggleFace();
        }
        completedActionPhases = -1;
        //mixes up the order of three phases so all three can be played before going to full randomness
        for (int i = 0; i < firstPhases.Count; i++) {
            int temp = firstPhases[i];
            int randomIndex = Random.Range(i, firstPhases.Count);
            firstPhases[i] = firstPhases[randomIndex];
            firstPhases[randomIndex] = temp;
        }

        lava.SetActive(false);
        transform.localScale = smallScale;
        StopCoroutine(co);
        auxAudioSource.Stop();
        auxAudioSource.loop = false;
        //begins the game anew
        LoadPhase(1);
        Time.timeScale = 1;
    }
}
