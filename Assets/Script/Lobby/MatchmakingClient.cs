using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Mirror.Examples.BilliardsPredicted;
using Mirror.Examples.Basic;


public class WebSocketClient : MonoBehaviour
{
    private readonly ClientWebSocket ws = new();
    private readonly Uri serverUri = new("ws://172.20.10.3:8090");    
    public LobbyUI uiInteract;
    public loader loader;
    private async void Start()
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
            if (parts.Length >= 2)
            {
                string LobbyId = parts[1];
                uiInteract.CodeDisplayer.GetComponent<Text>().text = "Code: " + LobbyId;
                uiInteract.DisplayLobbyChoice();
            }
        }

        // Exemple : "MatchFound 127.0.0.1:7777"
        if (message.StartsWith("MatchFound"))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2)
            {
                string address = parts[1];
                Debug.Log("🎮 Connexion à la partie sur : " + address);

                CustomNetworkManager.singleton.networkAddress = address;
                CustomNetworkManager.singleton.StartClient();
            }
        }
        if (message.StartsWith("GetAccount"))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 4)
            {
                string token = parts[1];
                string username = parts[2];
                string difficulties = parts[3];
                PlayerPrefs.SetString("id", token);
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.SetString("difficulties", difficulties);
                PlayerPrefs.Save();
            }
        }
    }

    public async void SendCreateLobby() => await SendMessage("CreateLobby " + PlayerPrefs.GetString("id"));

    public async void SendJoinLobby() => await SendMessage("JoinLobby " + uiInteract.joinLobbyButton
                                                            .transform.Find("Input").GetComponentInChildren<InputField>().text);
                                                            
    public async void SendConnection() => await SendMessage("Connection " + loader.FormConnect.transform.Find("InputUsername").GetComponentInChildren<InputField>().text + " " +
                                            loader.FormConnect.transform.Find("InputPassword").GetComponentInChildren<InputField>().text);
    public async void SendEnterMatchmaking() => await SendMessage("EnterMatchmaking");

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
