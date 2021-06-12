using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    private List<PlayerController> players;

    private void Awake()
    {
        players = new List<PlayerController>();
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PlayerController playerController = input.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlayerID = input.playerIndex;
            playerController.AssignLayers(input.playerIndex);
            playerController.Die.AddListener(OnPlayerDie);
            players.Add(playerController);
            VirtualAudioPlayer.SharedInstance.listeners.Add(playerController.transform);
            foreach (PlayerController p in players)
            {
                p.UpdateCanvasScale();
            }
        }
    }

    private void OnPlayerDie(int index)
    {
        print($"Player #{index} has died!");
        players[index].Disable();
        StartCoroutine(Respawn(index));
    }

    private IEnumerator Respawn(int index)
    {
        yield return new WaitForSeconds(3f);
        players[index].gameObject.SetActive(true);
        players[index].Respawn(new Vector3(0f, 50f, 0f), Vector3.zero);
    }
}
