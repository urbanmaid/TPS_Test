using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class NetworkRunnerManager : MonoBehaviour
{
    public static NetworkRunnerManager instance;

    [Header("Network Preference")]
    public bool isUsingNetwork = true;
    [Space]
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    public GameMode userGameMode = GameMode.AutoHostOrClient;
    public string userSessionName = "TestRoom";
    public string connectedUserName = "";

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("Standby for Network Connection..");
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 전환 감지! 새 씬: {scene.name}");
    }

    public async void StartConnection()
    {
        // If not using network, don't load network service...
        if(!isUsingNetwork) return;

        // NetworkRunner 인스턴스 생성
        NetworkRunner runner = Instantiate(networkRunnerPrefab);
        runner.ProvideInput = true; // 클라이언트가 입력을 제공하도록 설정

        // Set userSessionName if Empty
        if(userSessionName == "") userSessionName = "TestRoom";

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
                PlayerSpawner.Instance.SetRunnerAndInit(connectedUserName);
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