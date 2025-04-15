using UnityEngine;
using Fusion;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;
    [SerializeField] private NetworkObject playerPrefab;

    void Start()
    {
        Instance = this;
    }

    public void SetRunnerAndInit(string connectedUserName)
    {
        // NetworkRunner가 준비되면 플레이어 스폰
        var runner = FindAnyObjectByType<NetworkRunner>();
        if (runner != null && runner.IsRunning)
        {
            SpawnPlayer(runner, connectedUserName);
        }
        else if(runner == null)
        {
            Debug.LogError("There are no Network Object Connected players on this side. Spawnning...");
        }
    }

    public void SpawnPlayer(NetworkRunner runner, string userName)
    {
        //Debug.Log($"Runner 상태: IsRunning={runner.IsRunning}, IsServer={runner.IsServer}, IsClient={runner.IsClient}");

        // 서버 또는 호스트일 때만 스폰
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {
            NetworkObject player = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, PlayerRef.None);
            Debug.Log("플레이어 스폰 완료! 서버/호스트에서 처리했어~!");

            // 클라이언트 접속 메시지 전송
            PlayerCommunication comm = player.GetComponent<PlayerCommunication>();
            if (comm != null)
            {
                comm.SetUserName(userName);
                comm.RPC_SendMessage($"호스트: 새로운 플레이어({comm.userName})가 접속했어!");
            }
        }
        else
        {
            Debug.Log("클라이언트는 스폰 안 해! 서버가 할 거야~");
        }
    }
}