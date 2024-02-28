using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerScriptManager : NetworkBehaviour
{
    UIController _uiScript;
    LobbyPlayer _lobbyScript;
    PlayerAction _actionScript;

    private void Awake()
    {
        _uiScript = GetComponent<UIController>();
        _lobbyScript = GetComponent<LobbyPlayer>();
        _actionScript = GetComponent<PlayerAction>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if(IsSpawned)
        {
            Debug.Log(NetworkObjectId +  " ! I am not the server");
        }
        
        /*if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _uiScript.enabled = true;
            _actionScript.enabled = true;
            _lobbyScript.enabled = false;
            Debug.Log("Player script manager active");
        }
        else
        {
            _uiScript.enabled = false;
            _actionScript.enabled = false;
            _lobbyScript.enabled = true;
            Debug.Log("Player script manager active");
        }

        SetUpPlayerRpc();*/
    }

    
   public void SetUpPlayerRpc()
    {
        _uiScript = GetComponent<UIController>();
        _lobbyScript = GetComponent<LobbyPlayer>();
        _actionScript = GetComponent<PlayerAction>();

        _uiScript.enabled = true;
        _actionScript.enabled = true;
        _lobbyScript.enabled = false;
        Debug.Log("Player script manager active");
    }
}
