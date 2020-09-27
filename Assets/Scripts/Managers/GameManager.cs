using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player;

    public AudioClip bgm;
    public AudioClip deathSound;

    private AudioSource source;


    private bool notifiedDead = false;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        source.clip = bgm;
        source.Play();
        source.loop = true;
        // play music start here?
        notifiedDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(!notifiedDead && player.GetComponent<Animator>().GetBool("IsDying"))
        {
            
            OnPlayerDeath();
        }
    }


    void OnPlayerDeath()
    {
        Debug.Log("muerte");
        notifiedDead = true;
        // play death music
        source.clip = deathSound;
        source.Play();
        source.loop = false;
    }

}
