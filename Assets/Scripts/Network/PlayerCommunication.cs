using UnityEngine;
using Fusion;

public class PlayerCommunication : NetworkBehaviour
{
    [Header("Communication Infos")]
    public string userName;

    public override void Spawned() // Call when the NetworkObject has been spawned
    {
        if (userName == "") 
            userName = "Player_" + Random.Range(1000, 9999);
        RPC_SendMessage("접속 완료! 난 " + userName + "이야!");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    internal void RPC_SendMessage(string message)
    {
        Debug.Log("메시지 받았어: " + message);
    }
}