using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LobbyToGame : MonoBehaviour
{
    private bool isInLobby;

    public void StartGame()
    {
        if (isInLobby)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}
