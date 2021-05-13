using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private LayerMask defaultMask;
    private List<PlayerInput> players;

    public void OnPlayerJoined(PlayerInput input)
    {
        Camera cam = input.GetComponentInChildren<Camera>();
        cam.cullingMask = defaultMask;
    }
}
