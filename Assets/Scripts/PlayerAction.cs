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
    float targetPosition;
    float returnPosition;

    UIController uiScript;

    enum Action
    {
        Attack,
        Block,
        Feint
    }

    public enum Status
    {
        Standby,
        Choosing,
        Acting
    }

    //temmp
    bool gaming; 

    Action playerChoice;

    public Status status;

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
        gm.GetPlayer(gameObject);
        uiScript = GetComponent<UIController>();
        id = (int)NetworkObjectId;

        health = 5;
        targetPosition = 0;
        status = Status.Standby;


        if(IsHost)
        {
            transform.position = new Vector2(-2f, -1f);
            returnPosition = -2f;
        } else
        {
            transform.position = new Vector2(2f, -1f);
            returnPosition = 2f;
        }

    }

    // Update is called once per frame
    private void Update()
    {

        switch (status)
        {
            case Status.Standby:
                break;
            case Status.Choosing:
                uiScript.ShowCountdown();
                
                break;
            case Status.Acting:
                MoveToOpp();
                break;
        }

        /*if (!IsOwner)
        {
            return;
        }*/

        //transform.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 0.1f);

        if (Input.GetKeyDown(KeyCode.F) && !gaming)
        {
            targetPosition = 0;
            
            canAct = true;
            gaming = true;
            //CallCooldownRpc();
            StartCountdownRpc();
        }

        if(canAct)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerChoice = Action.Attack;
                //StartMoveRpc("attacking");
                canAct = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerChoice = Action.Block;
                //StartMoveRpc("attacking");
                canAct = false;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                playerChoice = Action.Feint;
                //StartMoveRpc("attacking");
                canAct = false;
            }


        }    

    }


    [Rpc(SendTo.Everyone)]
    public void StartMoveRpc(string command)
    {
        status = Status.Acting;
        
    }

    [Rpc(SendTo.Everyone)] 
    void StartCountdownRpc()
    {
        status = Status.Choosing;
    }

    [Rpc(SendTo.NotServer)]
    void StartMoveClientRpc(string command)
    {
        //Debug.Log(command);
        status = Status.Acting;
    }

    void MoveToOpp()
    {
       
        transform.position = Vector2.Lerp(transform.position, new Vector2(targetPosition, -1f), 0.3f);

        if(Vector2.Distance(transform.position, new Vector2(targetPosition,-1f)) < 0.1f)
        {
            status = Status.Standby;

            StartCooldown();
            
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

    

    [Rpc(SendTo.Server)]
    void CallCooldownServerRpc()
    {
       
        IEnumerator coroutine = Countdown();
        StartCoroutine(coroutine);
        //CallCooldownClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void CallCooldownClientRpc()
    {
        IEnumerator coroutine = Countdown();
        StartCoroutine(coroutine);
    }

    void StartCooldown()
    {
        IEnumerator coroutine = Countdown();
        StartCoroutine(coroutine);
    }
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(0.5f);

        //Debug.Log(playerChoice);

        

        if (Vector2.Distance(transform.position, new Vector2(0, -1f)) < 0.1f)
        {
            GetOutcome();
            targetPosition = returnPosition;
            //StartMoveRpc("returning");
            status = Status.Acting;
            //status = Status.Standby;
        } else
        {
            targetPosition = 0;
            yield return new WaitForSeconds(1f);

            StartCountdownRpc();

            canAct = true;
        }


        
        
    }

    void GetOutcome()
    {
        if (IsHost)
        {
            CheckForWinnerClientRpc(playerChoice);
        }
        else
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
            uiScript.UpdateHealth(health);
        }
        //Debug.Log(playerChoice);
    }

    [Rpc(SendTo.NotServer)]
    void CheckForWinnerClientRpc(Action otherChoice)
    {
        

        if (playerChoice == actionCheck[otherChoice])
        {
            health--;
            uiScript.UpdateHealth(health);
        }
        //Debug.Log(playerChoice);
    }

}
