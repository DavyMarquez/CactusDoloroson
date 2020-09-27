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

    private float timeSinceLastSpawn = 5.0f;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        timeSinceLastSpawn = timeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
       
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            

            timeSinceLastSpawn = 0.0f;
            for (int i = 0; i < spawnAmount; ++i)
            {
                if (VisibleByCamera())
                {
                    return;
                }

                float rand = Random.Range(0.0f, 1.0f);
                if (rand < bearChance)
                {
                    Instantiate(enemyTypes[3], transform.position, Quaternion.identity);
                    continue;
                }
                if (rand < skunkChance)
                {
                    Instantiate(enemyTypes[2], transform.position, Quaternion.identity);
                    continue;
                }
                if (rand < tortoiseChance)
                {
                    Instantiate(enemyTypes[1], transform.position, Quaternion.identity);
                    continue;
                }
                if (rand < puppyChance)
                {
                    Instantiate(enemyTypes[0], transform.position, Quaternion.identity);
                    continue;
                }

            }
        }
    }

    public bool VisibleByCamera()
    {
        if (!cam)
        {
            return false;
        }
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        if (viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1)
        {
            timeSinceLastSpawn = 0.0f;
            return true;
        }
        return false;
    }
}
