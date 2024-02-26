using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ParrelSync;

public class PlayerAction : NetworkBehaviour
{
    public bool canAct;
    GameManager gm;
    int id;
    [SerializeField] GameObject m_circle;

    public int health; 

    enum Action
    {
        Attack,
        Block,
        Feint
    }

    Action playerChoice;

    Dictionary<KeyCode, Action> inputCheck = new Dictionary<KeyCode, Action>()
    {
        {KeyCode.UpArrow, Action.Attack },
        {KeyCode.LeftArrow, Action.Block },
        {KeyCode.RightArrow, Action.Feint }
    };


    Dictionary<Action, Action> actionCheck = new Dictionary<Action, Action>() {
        {Action.Attack, Action.Feint },
        {Action.Block, Action.Attack },
        {Action.Feint, Action.Block }
    };


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        id = (int)NetworkObjectId;

        health = 5; 

        if(IsHost)
        {
            transform.position = new Vector2(-5f, -1f);
        } else
        {
            transform.position = new Vector2(5f, -1f);
        }

    }

    // Update is called once per frame
    private void Update()
    {
        
        if(!IsOwner)
        {
            return;
        }

        transform.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 0.1f);

        if (Input.GetKeyDown(KeyCode.F))
        {
            canAct = true;
            CallCooldownRpc();
        }

        if(canAct)
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                playerChoice = Action.Attack;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                playerChoice = Action.Block;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                playerChoice = Action.Feint;
            }*/

            if(Input.anyKeyDown)
            {
                Debug.Log(Event.current.keyCode);
               // playerChoice = inputCheck[Event.current.keyCode];
            }
        }

    }

    [Rpc(SendTo.Server)]
    void TestSpawnRpc(Color color)
    {
        
        // We're first instantiating the new instance in the host client
        GameObject newGameObject = Object.Instantiate(m_circle, new Vector2(0, 0), Quaternion.identity);
        
        // Replicating that same new instance to all connected clients
        NetworkObject newGameObjectNetworkObject = newGameObject.GetComponent<NetworkObject>();
        newGameObjectNetworkObject.Spawn(true);
    }

    

    [Rpc(SendTo.ClientsAndHost)]
    void CallCooldownRpc()
    {
       
        IEnumerator coroutine = CountdownRpc();
        StartCoroutine(coroutine);
    }

    
    IEnumerator CountdownRpc()
    {
        yield return new WaitForSeconds(2f);
        canAct = false;
        //Debug.Log(playerChoice);

        if(IsHost)
        {
            CheckForWinnerClientRpc(playerChoice);
        } else
        {
            CheckForWinnerServerRpc(playerChoice);
        }
        
    }

    [Rpc(SendTo.Server)]
    void CheckForWinnerServerRpc(Action otherChoice)
    {
        
        if(playerChoice == actionCheck[otherChoice])
        {
            health--;
        }
        Debug.Log(health);
    }

    [Rpc(SendTo.NotServer)]
    void CheckForWinnerClientRpc(Action otherChoice)
    {
        

        if (playerChoice == actionCheck[otherChoice])
        {
            health--;
        }
        Debug.Log(health);
    }

}
