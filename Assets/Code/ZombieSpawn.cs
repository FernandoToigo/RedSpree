using UnityEngine;
using System.Collections;

public class ZombieSpawn : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject flyingMonsterPrefab;

    private Scenario _scenario;

    void Start()
    {
        _scenario = GameObject.Find("Scenario Ground").GetComponent<Scenario>();
    }

    float ZombiesFunc(float x)
    {
        return Mathf.Cos(x / 10.0f + (-Mathf.PI / 2.0f)) * 0.4f + (x / 100.0f);
    }

    float FlyingMonstersFunc(float x)
    {
        return Mathf.Cos(x / 10.0f + (-Mathf.PI / 2.0f)) * 0.05f + (x / 300.0f);
    }

    void Update()
    {
        var doubleBulletsChance = 0.5f;
        if (Player.zombiesKilled >= 100)
            doubleBulletsChance = 0.10f;

        float zombieSpawnPerSecond = ZombiesFunc(Time.timeSinceLevelLoad);
        if (Random.Range(0.0f, 1.0f) < Time.deltaTime * zombieSpawnPerSecond)
        {
            GameObject o = (GameObject)Instantiate(zombiePrefab);
            var zombie = o.GetComponent<Zombie>();
            var bulletsRand = Random.value;
            zombie.bullets = bulletsRand < doubleBulletsChance ? 2 : 1;
            //zombie.speed = Random.Range(0.1f, 0.3f);
            zombie.speed = 0.3f;

            //o.transform.localPosition += new Vector3(Player.distanceTraveled + 18.0f, _scenario.GroundHeight, 0.0f);
            o.transform.localPosition = new Vector3(_scenario.LastPosition.x, _scenario.GroundHeight, 0.0f);
        }

        float flyingMonstersSpawnPerSecond = FlyingMonstersFunc(Time.timeSinceLevelLoad);
        if (Random.Range(0.0f, 1.0f) < Time.deltaTime * flyingMonstersSpawnPerSecond)
        {
            GameObject o = (GameObject)Instantiate(flyingMonsterPrefab);
            var flyingMonster = o.GetComponent<FlyingMonster>();
            flyingMonster.speed = Random.Range(7f, 10f);
            o.transform.localPosition += new Vector3(Player.distanceTraveled + 18.0f, Random.Range(3.0f, 6.5f), 0.0f);
        }

        //Debug.Log(string.Format("[{0}] [{1}]", zombieSpawnPerSecond, flyingMonstersSpawnPerSecond));
    }
}