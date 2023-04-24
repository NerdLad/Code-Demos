using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script for spawning fireballs upwards during the mall boss
public class FireSpawner : MonoBehaviour
{
    public GameObject fire;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaunchFireBall());
    }
    public IEnumerator LaunchFireBall() {
        for (int i = 0; i < 5; i++) {
            SpawnAndLaunchFireball();
            yield return new WaitForSeconds(.15f);
            SpawnAndLaunchFireball();

            yield return new WaitForSeconds(.15f);
            SpawnAndLaunchFireball();

            yield return new WaitForSeconds(.15f);
            SpawnAndLaunchFireball();

            yield return new WaitForSeconds(1);
            SpawnAndLaunchFireball();
        }
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
    //launches the fireballs
    void SpawnAndLaunchFireball() {
        var fireBall = Instantiate(fire, transform.position, Quaternion.identity);
        fireBall.GetComponent<Rigidbody2D>().AddForce(20 * transform.up * 100);
        fireBall.transform.parent = gameObject.transform.parent;
        Destroy(fireBall, 5);
    }
}
