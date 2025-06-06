using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Edgegap;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI singleton;

    [Header("UI Elements")]
    public Transform playerListRoot;
    public GameObject playerItemPrefab;
    public Button startGameButton;
    public GameObject joinLobbyButton;
    public GameObject leaveLobbyButton;
    public GameObject CodeDisplayer;
    public GameObject LobbyChoice;

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
}
