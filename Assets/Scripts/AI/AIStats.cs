using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIStats : MonoBehaviour
{
    [Range(-100.0f,100.0f)]
    public float love = 10.0f;
    public float Love
    {
        get { return love; }
        set { love = value; }
    }

    [Range(-100.0f, 100.0f)]
    public float sorrow = 10.0f;
    public float Sorrow
    {
        get { return sorrow; }
        set { sorrow = value; }
    }

}
