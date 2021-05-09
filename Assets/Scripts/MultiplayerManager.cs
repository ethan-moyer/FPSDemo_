using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int maxPlayers;
    [SerializeField] private Vector3 spawnPoint;
    private List<PlayerInputManager> players;

    /*private void Awake()
    {
        if (playerPrefab.GetComponent<PlayerInputManager>() == null)
        {
            Debug.LogWarning("PlayerPrefab has no PlayerInputManager component.");
            return;
        }

        for (int i = 0; i < maxPlayers; ++i)
        {
            PlayerInputManager newPlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity).GetComponent<PlayerInputManager>();
            if (i == 0)
            {
                newPlayer.AssignDevice(Keyboard.current);
                newPlayer.AssignDevice(Mouse.current);
            }
            else
            {
                if (Gamepad.all.Count >= maxPlayers - 1)
                {
                    newPlayer.AssignDevice(Gamepad.all[i - 1]);
                }
            }
        }
    }*/
}
