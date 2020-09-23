using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    // All ais in the scene
    [SerializeField]
    private List<GameObject> aiList = new List<GameObject>();

    public List<GameObject> AIList
    {
        get { return aiList; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAI(GameObject go)
    {
        aiList.Add(go);
    }

    public void RemoveAI(GameObject go)
    {
        aiList.Remove(go);
    }
}
