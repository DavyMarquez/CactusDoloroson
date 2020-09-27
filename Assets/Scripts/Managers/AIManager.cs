using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager
{
    private static AIManager instance = null;

    private int huggedPuppies = 0;

    public int HuggedPuppies
    {
        get { return huggedPuppies; }
        set { huggedPuppies = value; }
    }

    private int huggedSkunks = 0;
    public int HuggedSkunks
    {
        get { return huggedSkunks; }
        set { huggedSkunks = value; }
    }

    private int huggedTortoises = 0;
    public int HuggedTortoises
    {
        get { return huggedTortoises; }
        set { huggedTortoises = value; }
    }

    private int huggedBears = 0;
    public int HuggedBears
    {
        get { return huggedBears; }
        set { huggedBears = value; }
    }

    private int pickedFlowers = 0;
    public int PickedFlowers
    {
        get { return pickedFlowers; }
        set { pickedFlowers = value; }
    }

    private float timeOfGame = 0.0f;
    public float TimeOfGame
    {
        get { return timeOfGame; }
        set { timeOfGame = value; }
    }

    public static AIManager GetInstance()
    {
        if(instance == null)
        {
            instance = new AIManager();
        }
        return instance;
    }

    public void IncreaseHuggedPuppies()
    {
        huggedPuppies++;
        Debug.Log(huggedPuppies);
    }

    public void IncreaseHuggedSkunks()
    {
        huggedSkunks++;
        Debug.Log(huggedSkunks);
    }

    public void IncreaseHuggedTortoises()
    {
        huggedTortoises++;
        Debug.Log(huggedTortoises);
    }

    public void IncreaseHuggedBears()
    {
        huggedBears++;
        //Debug.Log(huggedBears);
    }

    public void IncreasePickedFlowers()
    {
        pickedFlowers++;
    }

}
