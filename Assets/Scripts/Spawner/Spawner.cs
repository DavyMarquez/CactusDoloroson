using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 0 - puppy, 1 - tortoise, 2 - skunk, 3 - bear
    public GameObject[] enemyTypes;

    [Range(0.0f,1.0f)]
    public float puppyChance = 0.5f;

    [Range(0.0f,1.0f)]
    public float tortoiseChance = 0.6f;

    [Range(0.0f,1.0f)]
    public float skunkChance = 0.75f;

    [Range(0.0f,1.0f)]
    public float bearChance = 0.85f;

    [Min(1)]
    public int spawnAmount = 3;

    [Min(0.0f)]
    public float timeBetweenSpawns = 5.0f;

    private float timeSinceLastSpawn = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastSpawn = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeSinceLastSpawn >= timeBetweenSpawns)
        {
            StartCoroutine(IncrementTimer());
            for(int i = 0; i < spawnAmount; ++i)
            {
                float rand = Random.Range(0.0f, 1.0f);
                if (rand < puppyChance)
                {
                    Instantiate(enemyTypes[0], transform.position, Quaternion.identity);
                }
                else if(rand < tortoiseChance)
                {
                    Instantiate(enemyTypes[1], transform.position, Quaternion.identity);
                }
                else if(rand < skunkChance)
                {
                    Instantiate(enemyTypes[2], transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(enemyTypes[3], transform.position, Quaternion.identity);
                }
            }
        }
    }

    IEnumerator IncrementTimer()
    {
        float timeAtEnter = Time.time;
        while (timeBetweenSpawns > Time.time - timeAtEnter)
        {
            timeSinceLastSpawn += Time.deltaTime;
            yield return null;

        }
        timeSinceLastSpawn = 0.0f;
    }
}
