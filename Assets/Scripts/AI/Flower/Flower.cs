using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{

    private Collider2D collider;

    private bool picked = false;

    [Min(0.0f)]
    public float respawnTime = 5.0f;

    private float timeSincePicked = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        timeSincePicked = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (picked)
        {
            timeSincePicked += Time.deltaTime;
        }
        if(timeSincePicked >= respawnTime)
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponent<Renderer>().enabled = true;
            picked = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.gameObject.tag == "Player")
        {
            picked = true;
            timeSincePicked = 0.0f;
            collision.transform.gameObject.GetComponent<PlayerStats>().IncreaseLove(GetComponent<AIStats>().Love);
            collision.transform.gameObject.GetComponent<PlayerStats>().DecreaseSorrow(GetComponent<AIStats>().Sorrow);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }
    }
}

