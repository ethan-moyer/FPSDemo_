using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private LayerMask defaultMask;
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
            playerController.AssignLayers(input.playerIndex);
            players.Add(playerController);
        }
    }
}
