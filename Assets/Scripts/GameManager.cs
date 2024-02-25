using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    GameObject p1, p2;
    // Start is called before the first frame update
    void Start()
    {
        p1 = NetworkManager.Singleton.ConnectedClients[0].PlayerObject.gameObject;
        p2 = NetworkManager.Singleton.ConnectedClients[1].PlayerObject.gameObject;
        

        p1.GetComponent<Transform>().position = new Vector2(-5, 0);
        p2.GetComponent<Transform>().position = new Vector2(5, 0);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
