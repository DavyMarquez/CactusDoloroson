using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private bool speedBuffNotified = false;
    private bool dashBuffNotified = false;

    private bool smell = false;

    //Audio
    private AudioSource source;
    public AudioClip loveClip;
    public AudioClip sorrowClip;
    public AudioClip deathSound;
    //Fin del audio

    public GenericBar loveBar;

    public GenericBar sorrowBar;

    [Min(0.0f)]
    public float timeToResetLoveBar = 10.0f;
    private AIManager aiManager;
    public bool Smell
    {
        get { return smell; }
        set { smell = value; }
    }

    private bool changedStinkStatus = false;

    [Range(0.0f, 100.0f)]
    public float speedBuffPercentage = 50.0f;

    [Range(0.0f, 100.0f)]
    public float dashBuffPercentage = 100.0f;

    private Animator animator;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float love = 0.0f;
    public float Love
    {
        get { return love; }
        set { love = value; }
    }

    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float sorrow = 0.0f;
    public float Sorrow
    {
        get { return sorrow; }
        set { sorrow = value; }
    }

    [SerializeField]
    private bool gameOver = true;

    [SerializeField]
    private float deathTime = 3.0f;

    public GameObject InLoveAnim;

    public GameObject SmellyAnim;

    // sorrow decreased by seconds
    [Range(0.0f, 100.0f)]
    public float sorrowIncreaseRate = 1.0f;

    private bool startedToIncreaseSorrow = false;

    // Since last hit or hug, time it'll begin to decrease sorrow
    [Min(0.0f)]
    public float timeToIncreaseSorrow = 5.0f;

    // time since last interaction
    private float timeSinceLastInteraction = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        love = 0.0f;
        sorrow = 0.0f;
        speedBuffNotified = false;
        dashBuffNotified = false;
        smell = false;
        animator = gameObject.GetComponent<Animator>();
        source = gameObject.GetComponent<AudioSource>();

        /*aiManager = FindObjectOfType<AIManager>();
        if (aiManager == null)
        {
            Debug.LogError("No AIManager found in scene");
        }*/

        loveBar.SetMax(100, love);
        sorrowBar.SetMax(100, sorrow);
    }

    // Update is called once per frame
    void Update()
    {
        if(smell && !changedStinkStatus)
        {
            changedStinkStatus = true;
            SmellyAnim.GetComponent<Animator>().SetBool("IsSmelling", true);
        }
        if(!smell && changedStinkStatus)
        {
            SmellyAnim.GetComponent<Animator>().SetBool("IsSmelling", false);
            changedStinkStatus = false;
        }
        //GridAI.GetInstance().ShowGrid();

        UpdateNotifications();

        timeSinceLastInteraction += Time.deltaTime;
        if(timeSinceLastInteraction > timeToIncreaseSorrow)
        {
            if (!startedToIncreaseSorrow)
            {
                startedToIncreaseSorrow = true;
                // PLAY AUDIO HERE
            }
            sorrow = Mathf.Min(sorrow + sorrowIncreaseRate * Time.deltaTime, 100.0f);
            sorrowBar.SetValue(sorrow);
            //ESTE NO TIRA
            source.clip = sorrowClip;
            source.Play();
        }
        if (sorrow >= 100.0f && !animator.GetBool("IsDying") && gameOver)
        {
            StartCoroutine(GameOver(deathTime));
        }

        Notify();
    }

    public void IncreaseLove(float amount)
    {
        //love = Mathf.Min(amount + love, 100.0f);
        if(love >= 100.0f)
        {
            return;
        }
        love = Mathf.Clamp(amount + love, 0.0f, 100.0f);
        loveBar.SetValue(love);
        if(love >= 100.0f)
        {
            StartCoroutine(ResetLoveBar());
        }
    }

    public void DecreaseLove(float amount)
    {
        //love = Mathf.Max(love - amount, 0.0f);
        love = Mathf.Clamp(love - amount, 0.0f, 100.0f);
        loveBar.SetValue(love);
    }

    public void IncreaseSorrow(float amount)
    {
        //sorrow = Mathf.Min(amount + sorrow, 100.0f);
        sorrow = Mathf.Clamp(sorrow + amount, 0.0f, 100.0f);
        sorrowBar.SetValue(sorrow);
    }

    public void DecreaseSorrow(float amount)
    {
        //sorrow = Mathf.Max(sorrow - amount, 0.0f);
        sorrow = Mathf.Clamp(sorrow - amount, 0.0f, 100.0f);
        sorrowBar.SetValue(sorrow);
    }

    public void TimeSinceLastInteractionReset()
    {
        timeSinceLastInteraction = 0.0f;
        startedToIncreaseSorrow = false;
    }

    void UpdateNotifications()
    {
        if(love < speedBuffPercentage)
        {
            speedBuffNotified = false;
        }
        if(love < dashBuffPercentage)
        {
            dashBuffNotified = false;
        }
    }

    void Notify()
    {
        if (love >= speedBuffPercentage && !speedBuffNotified)
        {
            speedBuffNotified = true;
            gameObject.GetComponent<PlayerMovement>().ApplySpeedBuff();
            source.clip = loveClip;
            source.Play();
        }
        if (love >= dashBuffPercentage && !dashBuffNotified)
        {
            dashBuffNotified = true;
            gameObject.GetComponent<Hug>().ApplyDashBuff();
            source.clip = loveClip;
            source.Play();
        }
    }

    IEnumerator GameOver(float timeToWait)
    {
        Destroy(gameObject.GetComponent<PlayerMovement>());
        source.clip = deathSound;
        source.Play();
        animator.SetBool("IsDying", true);
        GetComponent<Rigidbody2D>().MovePosition(transform.position);
        float timeAtStart = Time.time;
        AIManager.GetInstance().TimeOfGame = timeAtStart;
        Debug.Log(AIManager.GetInstance().TimeOfGame);
        while(timeToWait > Time.time - timeAtStart)
        {
            yield return null;
        }

    } 
    
    IEnumerator ResetLoveBar()
    {
        InLoveAnim.GetComponent<Animator>().SetBool("IsInLove", true);
        float timeAtStart = Time.time;
        while(timeToResetLoveBar > Time.time - timeAtStart)
        {
            yield return null;
        }
        love = 0.0f;
        InLoveAnim.GetComponent<Animator>().SetBool("IsInLove", false);
    }

    public void RemoveSmellInTime(float smellTimer)
    {
        StartCoroutine(RemoveSmell(smellTimer));

    }

    public IEnumerator RemoveSmell(float smellTimer)
    {
        float timeAtStart = Time.time;
        while (smellTimer > Time.time - timeAtStart)
        {
            yield return null;
        }
        Debug.LogWarning("removing smell ");
        setAvoidingPlayer(false);
    }

    public void setAvoidingPlayer(bool value)
    {
        smell = value;
        Debug.LogWarning("setting the variable " + value);
        
        /*if (aiManager != null)
        {
            foreach (GameObject p in aiManager.AIList)
            {
                if (p.TryGetComponent<AIStats>(out var aiStats))
                {
                    aiStats.IsAvoidingPlayer = value;
                }
            }
        }*/
    }
}
