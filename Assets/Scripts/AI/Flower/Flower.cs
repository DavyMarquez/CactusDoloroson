using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{

    private Collider2D col;

    private bool picked = false;

    [Min(0.0f)]
    public float respawnTime = 5.0f;

    private float timeSincePicked = 0.0f;

    [SerializeField]
    private float timeToFlip = 3.0f;

    private bool isFlipping = false;

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
        if (!isFlipping)
        {
            StartCoroutine(FlipIdle(Random.Range(timeToFlip-1, timeToFlip+1)));
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.gameObject.tag == "Player")
        {
            picked = true;
            timeSincePicked = 0.0f;
            collision.transform.gameObject.GetComponent<PlayerStats>().IncreaseLove(GetComponent<AIStats>().Love);
            collision.transform.gameObject.GetComponent<PlayerStats>().IncreaseSorrow(GetComponent<AIStats>().Sorrow);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }
    }

    IEnumerator FlipIdle(float WaitTime)
    {
        isFlipping = true;
        float timeAtStart = Time.time;
        while(WaitTime > Time.time - timeAtStart)
        {
            yield return null;
        }
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
        isFlipping = false;
    }
}

