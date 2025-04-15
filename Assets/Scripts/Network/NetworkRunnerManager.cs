using UnityEngine;
using Fusion;

public class NetworkRunnerManager : MonoBehaviour
{
    
    [Header("Network Preference")]
    [SerializeField] bool isUsingNetwork = true;
    [Space]
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private GameMode userGameMode = GameMode.AutoHostOrClient;
    [SerializeField] private string userSessionName = "TestRoom";

    void Start()
    {
        if(isUsingNetwork)
        {
            StartConnection();
        }
        else
        {
            Debug.Log("This session do not use Network.");
        }
    }

    async void StartConnection()
    {
        // NetworkRunner 인스턴스 생성
        NetworkRunner runner = Instantiate(networkRunnerPrefab);
        runner.ProvideInput = true; // 클라이언트가 입력을 제공하도록 설정

        // 게임 모드와 세션 설정
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = userGameMode, // 게임 모드 인스펙터에서 선택 (빌드 버전은 기본적으로 호스트로 플레이)
            SessionName = userSessionName
        });

        if (result.Ok)
        {
            Debug.Log($"방 연결 성공! 세션 이름: {userSessionName}, 모드: {userGameMode}");
            if (userGameMode == GameMode.Host)
            {
                PlayerSpawner.Instance.SetRunnerAndInit();
            }
            else if (userGameMode == GameMode.Client)
            {
                Debug.Log("클라이언트 모드로 방 접속.");
            }
        }
        else
        {
            Debug.LogError($"방 연결 실패: {result.ErrorMessage}");
        }
    }

    void OnApplicationQuit()
    {
        var runner = FindAnyObjectByType<NetworkRunner>();
        if (runner != null)
        {
            runner.Shutdown();
            Debug.Log("네트워크 종료 완료!");
        }
    }
}