using Unity.Netcode;
using UnityEngine;
using ParrelSync;
public class RpcTest : NetworkBehaviour
{
   
    void Start()
    {
        //Is this unity editor instance opening a clone project?
        if (ClonesManager.IsClone())
        {
            Debug.Log("This is a clone project.");
            // Get the custom argument for this clone project.  
            string customArgument = ClonesManager.GetArgument();
            // Do what ever you need with the argument string.
            Debug.Log("The custom argument of this clone project is: " + customArgument);
        }
        else
        {
            Debug.Log("This is the original project.");
        }

        

        
    }

    

    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            gameObject.GetComponent<Transform>().position = new Vector2(5, 0);
            TestServerRpc(0, NetworkObjectId);
        }

        if(IsServer) {
            gameObject.GetComponent<Transform>().position = new Vector2(-5, 0);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && IsOwner)
        {
            TestMoveRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestMoveRpc()
    {
        gameObject.GetComponent<Transform>().position = new Vector2(0, 0);

    }
}
