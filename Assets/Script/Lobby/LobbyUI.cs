using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;
using Mirror.SimpleWeb;
using Mirror.Examples.Basic;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI singleton;

    [Header("UI Elements")]
    public Transform playerListRoot;
    public GameObject playerItemPrefab;
    public Button startGameButton;
    public Button EnterMatchmaking;
    public GameObject joinLobbyButton;
    public GameObject createLobbyButton;
    public GameObject leaveLobbyButton;
    public GameObject CodeDisplayer;
    public GameObject LobbyChoice;
    public GameObject ErrorDisplayer;
    public static string characterChoice;

    void Awake()
    {
        // Singleton pattern
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        // Ajout du listener pour le bouton Start Game
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGameClicked);

        LobbyChoice.SetActive(false);
        WebSocketClient webSocketClient = GameObject.Find("WebSocketManager").GetComponent<WebSocketClient>();
        joinLobbyButton.GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendJoinLobby());
        createLobbyButton.GetComponent<Button>().onClick.AddListener(() => webSocketClient.SendCreateLobby());
        EnterMatchmaking.onClick.AddListener(() => webSocketClient.SendEnterMatchmaking());
        playerItemPrefab.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("username");
    }
    
    
    void OnStartGameClicked()
    {
        CustomNetworkManager.singleton.StartGame();
    }

    /// <summary>
    /// Met à jour l'affichage de la liste des joueurs dans le lobby.
    /// </summary>
    public void RefreshPlayerList()
    {
        if (playerListRoot == null || playerItemPrefab == null) return;

        // Supprime tous les anciens items
        foreach (Transform child in playerListRoot)
        {
            Destroy(child.gameObject);
        }

        List<LobbyPlayer> players = CustomNetworkManager.singleton.lobbyPlayers;
        foreach (var player in players)
        {
            GameObject item = Instantiate(playerItemPrefab, playerListRoot);
            Text label = item.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = player.isLocalPlayer ? "You" : "Player";
            }
        }
    }

    /// <summary>
    /// Met à jour la visibilité des boutons en fonction du nombre de joueurs.
    /// </summary>
    public void ChangeButtonIsInLobby()
    {
        int playerCount = CustomNetworkManager.singleton.lobbyPlayers.Count;

        bool inLobbyWithOthers = playerCount > 1;

        if (joinLobbyButton != null)
            joinLobbyButton.SetActive(!inLobbyWithOthers);

        if (leaveLobbyButton != null)
            leaveLobbyButton.SetActive(inLobbyWithOthers);
    }
    public void ToggleButtonState(GameObject button)
    {
        if (button != null)
            button.SetActive(!button.activeSelf);
    }
    public void DisplayLobbyChoice()
    {
        LobbyChoice.SetActive(!LobbyChoice.activeSelf);
    }
    public void ChangeCharacter(Sprite newCharacterSprite)
    {
        if (playerItemPrefab != null)
        {
            Image img = playerItemPrefab.GetComponent<Image>();
            if (img != null && newCharacterSprite != null)
            {
                img.sprite = newCharacterSprite;
            }
        }
    }
    public void ChangeCharacterStock(int index)
    {
        if (playerItemPrefab != null)
        {
            PlayerPrefs.SetInt("indexChar", index);
            PlayerPrefs.Save();
        }
    }
}
