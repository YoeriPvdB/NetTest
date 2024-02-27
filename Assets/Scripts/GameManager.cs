using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    

    public enum Status
    {
        Standby,
        Choosing,
        Acting
    }

    public Status status;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        if(status == Status.Acting)
        {
            //Debug.Log("we are acting");
            MovePlayerRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void MovePlayerRpc()
    {
        Debug.Log(player.GetComponent<NetworkObject>().NetworkObjectId + " is here");
        player.GetComponent<PlayerAction>().status = PlayerAction.Status.Acting;
        player.transform.position = Vector2.Lerp(player.transform.position, new Vector2(0, -1f), 0.01f);

        if (Vector2.Distance(player.transform.position, new Vector2(0, -1f)) < 0.1f)
        {
            status = Status.Standby;
        }
    }

    public void GetPlayer(GameObject playerGO)
    {
        player = playerGO;
    }

    
    

    
}

