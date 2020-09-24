using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private bool speedBuffNotified = false;
    private bool dashBuffNotified = false;

    private bool smell = false;

    private AIManager aiManager;
    public bool Smell
    {
        get { return smell; }
        set { smell = value; }
    }


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

    // sorrow decreased by seconds
    [Range(0.0f, 100.0f)]
    public float sorrowIncreaseRate = 1.0f;

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
        aiManager = FindObjectOfType<AIManager>();
        if (aiManager == null)
        {
            Debug.LogError("No AIManager found in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNotifications();

        timeSinceLastInteraction += Time.deltaTime;
        if(timeSinceLastInteraction > timeToIncreaseSorrow)
        {
            sorrow = Mathf.Min(sorrow + sorrowIncreaseRate * Time.deltaTime, 100.0f);
        }
        if (sorrow >= 100.0f && !animator.GetBool("IsDying") && gameOver)
        {
            StartCoroutine(GameOver());
        }

        Notify();
    }

    public void IncreaseLove(float amount)
    {
        //love = Mathf.Min(amount + love, 100.0f);
        love = Mathf.Clamp(amount + love, 0.0f, 100.0f);
    }

    public void DecreaseLove(float amount)
    {
        //love = Mathf.Max(love - amount, 0.0f);
        love = Mathf.Clamp(love - amount, 0.0f, 100.0f);
    }

    public void IncreaseSorrow(float amount)
    {
        //sorrow = Mathf.Min(amount + sorrow, 100.0f);
        sorrow = Mathf.Clamp(sorrow + amount, 0.0f, 100.0f);
        timeSinceLastInteraction = 0.0f;
    }

    public void DecreaseSorrow(float amount)
    {
        //sorrow = Mathf.Max(sorrow - amount, 0.0f);
        sorrow = Mathf.Clamp(sorrow - amount, 0.0f, 100.0f);
        timeSinceLastInteraction = 0.0f;
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
        }
        if (love >= dashBuffPercentage && !dashBuffNotified)
        {
            dashBuffNotified = true;
            gameObject.GetComponent<Hug>().ApplyDashBuff();
        }
    }

    IEnumerator GameOver()
    {
        Destroy(gameObject.GetComponent<PlayerMovement>());
        animator.SetBool("IsDying", true);
        GetComponent<Rigidbody2D>().MovePosition(transform.position);
        float timeAtStart = Time.time;
        while(3 > Time.time - timeAtStart)
        {
            yield return null;
        }
        Debug.Log("Muerto");
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
        
        if (aiManager != null)
        {
            foreach (GameObject p in aiManager.AIList)
            {
                if (p.GetComponent<AIStats>() != null)
                {
                    p.GetComponent<AIStats>().IsAvoidingPlayer = value;
                }
            }
        }
    }
}
