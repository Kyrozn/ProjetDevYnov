using System.Collections;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public enum GameState
{
    Loading,
    Lobby,
    Game,
    Defeat,
    Victory,
    Matchmaking,
    IDK
}

public class PersistentObject : MonoBehaviour
{
    public GameState currentState = GameState.IDK;
    private WebSocketClient webSocketClient;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (FindObjectsByType<PersistentObject>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        webSocketClient = GetComponent<WebSocketClient>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        switch (scene.name)
        {
            case "LoadingScreen":
                currentState = GameState.Loading;
                webSocketClient.loader = GameObject.Find("Image").GetComponent<Loader>();
                GameObject.Find("Video Player").GetComponent<VideoPlayer>().targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                webSocketClient.uiInteract = null;
                GameObject.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendConnection());
                break;
            case "Lobby":
                currentState = GameState.Lobby;
                webSocketClient.loader = null;
                webSocketClient.uiInteract = GameObject.Find("Canvas").GetComponent<LobbyUI>();
                GameObject.Find("LuffyChoice").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendChangeCharacter("Luffy"));
                GameObject.Find("ZoroChoice").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendChangeCharacter("Zoro"));
                GameObject.Find("SanjiChoice").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendChangeCharacter("Sanji"));
                GameObject.Find("UssopChoice").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendChangeCharacter("Ussop"));
                GameObject.Find("Video Player").GetComponent<VideoPlayer>().targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                GameObject.Find("NetworkManager").GetComponent<KcpTransport>().Port = (ushort)PlayerPrefs.GetInt("port");
                break;
            case "Game":
                currentState = GameState.Game;
                webSocketClient.loader = null;
                webSocketClient.uiInteract = null;
                GameObject.Find("Main Camera").SetActive(false);
                break;
            case "MatchmakingLoading":
                currentState = GameState.Matchmaking;
                webSocketClient.loader = null;
                webSocketClient.uiInteract = null;
                GameObject.Find("LeftMatchmakingButton").GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendLeftMatchmaking());
                break;
            case "Defeat":
                currentState = GameState.Defeat;
                GameObject.Find("Video Player").GetComponent<VideoPlayer>().targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                StartCoroutine(DelayedDestroyContainer());
                break;
            case "Victory":
                currentState = GameState.Victory;
                GameObject.Find("Video Player").GetComponent<VideoPlayer>().targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                StartCoroutine(DelayedDestroyContainer());
                break;
            default:
                currentState = GameState.IDK;
                break;
        }

        Debug.Log($"Nouvelle scène : {scene.name} / Nouvel état : {currentState}");

    }

    private IEnumerator DelayedDestroyContainer()
    {
        yield return new WaitForSeconds(3);
        webSocketClient.SendDestroyContainer();
    }
}