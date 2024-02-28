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
    float zoomDir;
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
        zoomDir = 3f;

        if(IsHost)
        {
            transform.position = new Vector2(-2f, -1f);
            returnPosition = -2f;
        } else
        {
            transform.position = new Vector2(2f, -1f);
            returnPosition = 2f;
        }

        /*if(IsOwner)
        uiScript.EnableSlider(false);*/

    }

    // Update is called once per frame
    private void Update()
    {

        switch (status)
        {
            case Status.Standby:
                //uiScript.ZoomCamera(5f);
                break;

            case Status.Choosing:

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playerChoice = Action.Attack;
                    uiScript.HighlightActionChoice(2);
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    playerChoice = Action.Block;
                    uiScript.HighlightActionChoice(1);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    playerChoice = Action.Feint;
                    uiScript.HighlightActionChoice(3);
                }

                uiScript.ShowCountdown();
                
                break;

            case Status.Acting:
                //uiScript.ZoomCamera(zoomDir);
                MoveToOpp();
                break;
        }

        if (Input.GetKeyDown(KeyCode.F) && !gaming)
        {
            targetPosition = 0;
            
            
            gaming = true;
            //CallCooldownRpc();
            StartCountdownRpc();
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
        uiScript.EnableSlider(true);
        zoomDir = 3;
        status = Status.Choosing;
    }

    

    void MoveToOpp()
    {
       
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetPosition, -1f), Time.deltaTime * 60f);

        if(Vector2.Distance(transform.position, new Vector2(targetPosition,-1f)) < 0.1f)
        {
            status = Status.Standby;

            StartCooldown();
            
        }
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
            zoomDir = 5;
            status = Status.Acting;
            
        } else
        {
            targetPosition = 0;
            yield return new WaitForSeconds(1f);
            
            StartCountdownRpc();

            status = Status.Choosing; 
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
