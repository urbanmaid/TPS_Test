using UnityEngine;
using Fusion;

public class PlayerCommunication : NetworkBehaviour
{
    [Header("Communication Infos")]
    public string userName;

    public override void Spawned() // Call when the NetworkObject has been spawned
    {
        RPC_SendMessage("New Player Has been spawned.");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    internal void RPC_SendMessage(string message)
    {
        Debug.Log(message);
    }

    internal void SetUserName(string userNameInput)
    {
        if (userNameInput == "") {
            userName = "Player_" + Random.Range(1000, 9999);
        }
        else
        {
            userName = userNameInput;
        }
        RPC_SendMessage(userName + " has reached into Session");
    }
}