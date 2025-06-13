using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;


public class WebSocketClient : MonoBehaviour
{
    private readonly ClientWebSocket ws = new();
    private readonly Uri serverUri = new("ws://localhost:8090");
    public LobbyUI uiInteract;
    public Loader loader;

    private async void Awake()
    {

        if (!PlayerPrefs.HasKey("id"))
        {
            await ws.ConnectAsync(serverUri, CancellationToken.None);
        }
        else
        {
            await ConnectToServer();
        }
        
        _ = ReceiveMessages(); // Start receiving without awaiting it
    }

    private async Task ConnectToServer()
    {
        try
        {
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("✅ Connecté au serveur WebSocket");

            // Exemple : envoyer un token d'identification (à remplacer dynamiquement)
            await SendMessage("CheckToken " + PlayerPrefs.GetString("id"));
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ WebSocket Connection Error: " + ex.Message);
            _ = ConnectToServer();
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024];

        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log("📨 Message reçu : " + message);

            HandleServerMessage(message);
        }
    }

    private void HandleServerMessage(string message)
    {
            if (message.StartsWith("LobbyCreated"))
            {
                string[] parts = message.Split(' ');
                if (parts.Length >= 3)
                {
                    string LobbyId = parts[1];
                    uiInteract.CodeDisplayer.GetComponent<Text>().text = "Code: " + LobbyId;
                    uiInteract.startGameButton.gameObject.SetActive(true);
                    uiInteract.EnterMatchmaking.gameObject.SetActive(false);
                    uiInteract.DisplayLobbyChoice();
                    PlayerPrefs.SetInt("port", int.Parse(parts[2]));
                    PlayerPrefs.Save();
                }
            }
            else if (message.StartsWith("GetAccount"))
            {
                string[] parts = message.Split(' ');
                if (parts.Length >= 4)
                {
                    Debug.LogWarning("test");
                    string token = parts[1];
                    string username = parts[2];
                    string difficulties = parts[3];
                    PlayerPrefs.SetString("id", token);
                    PlayerPrefs.SetString("username", username);
                    PlayerPrefs.SetString("difficulties", difficulties);
                    PlayerPrefs.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                }
            }
            else if (message.StartsWith("MatchmakingStart"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MatchmakingLoading");
            }
            else if (message.StartsWith("Erreur"))
            {
                uiInteract.ErrorDisplayer.SetActive(true);
                string[] words = message.Split(' ');
                string result = string.Join(" ", words.Skip(1));
                uiInteract.ErrorDisplayer.GetComponent<Text>().text = result; StartCoroutine(DelayedAction());

                IEnumerator DelayedAction()
                {
                    yield return new WaitForSeconds(3f);
                    uiInteract.ErrorDisplayer.SetActive(false);
                }
            }
    }

    public async void SendCreateLobby() => await SendMessage("CreateLobby " + PlayerPrefs.GetString("id"));

    public async void SendJoinLobby() => await SendMessage("JoinLobby " + uiInteract.joinLobbyButton
                                                            .transform.Find("Input").GetComponentInChildren<InputField>().text);
                                                    
    public async void SendConnection() => await SendMessage("Connection " + loader.FormConnect.transform.Find("InputUsername").GetComponentInChildren<InputField>().text + " " +
                                            loader.FormConnect.transform.Find("InputPassword").GetComponentInChildren<InputField>().text);
    public async void SendEnterMatchmaking() => await SendMessage("EnterMatchmaking " + LobbyUI.characterChoice);
    public async void SendStartGame() => await SendMessage("StartGame");
    public async void SendChangeCharacter(string characterChoiced) => await SendMessage("ChangeCharacter " + characterChoiced);
    public async void SendLeftMatchmaking() => await SendMessage("LeftMatchmaking");
    public async void SendDestroyContainer() => await SendMessage("DestroyContainer");

    private new async Task SendMessage(string msg)
    {
        if (ws.State != WebSocketState.Open)
        {
            Debug.LogWarning("WebSocket non connecté");
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(msg);
        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        Debug.Log("📤 Message envoyé : " + msg);
    }

    private async void OnApplicationQuit()
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermeture client", CancellationToken.None);
            Debug.Log("🔌 WebSocket fermé proprement");
        }
    }

}
