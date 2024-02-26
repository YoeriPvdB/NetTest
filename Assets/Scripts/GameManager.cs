using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    private IEnumerator cdCoroutine;

    public Dictionary<int,GameObject> players = new Dictionary<int,GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void AddPlayersRpc(GameObject pObject) 
    {
        Debug.Log("checking to add");
        Debug.Log(pObject);
        players.Add(players.Count+1, pObject);
        Debug.Log("added");
        if (players.Count == 2)
        {
            Debug.Log("full lobby ");
            StartCD(2f);
        }
        
    }

    public void StartCD(float timer)
    {
        cdCoroutine = BreakCooldownRpc(timer);
        StartCoroutine(cdCoroutine);
    }

    IEnumerator BreakCooldownRpc(float cdTimer)
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(cdTimer);

        
        //pAction.canMove = true;
        Debug.Log("moving");
    }

    
}

