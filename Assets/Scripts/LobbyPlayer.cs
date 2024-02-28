using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class LobbyPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
        if(!IsServer && IsSpawned)
        {
            InformHostRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Rpc(SendTo.Server)] 
    void InformHostRpc()
    {
        Debug.Log("Player 2 is here. " + NetworkObjectId);
        //NetworkManager.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        NetworkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        ChangeScriptsRpc();
        
    }

    [Rpc(SendTo.Everyone)]
    void ChangeScriptsRpc()
    {
        GetComponent<PlayerScriptManager>().SetUpPlayerRpc();
    }

   

    
}
