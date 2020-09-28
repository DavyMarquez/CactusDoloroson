using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    [SerializeField]
    private Text time;

    [SerializeField]
    private Text hugs;

    [SerializeField]
    private Text flowers;

    void Start()
    {
        time.text = AIManager.GetInstance().FormatTime();
        hugs.text = AIManager.GetInstance().TotalHugs().ToString();
        flowers.text = AIManager.GetInstance().PickedFlowers.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
