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
        Transform worldModel = input.transform.Find("Player Model");
        Transform viewModel = cam.transform.Find("View Model Pivot");
        cam.cullingMask = defaultMask;
        int worldLayer = 0;
        int viewLayer = 0;
        switch (input.playerIndex)
        {
            case 0:
                worldLayer = 17;
                viewLayer = 13;
                break;
            case 1:
                worldLayer = 18;
                viewLayer = 14;
                break;
            case 2:
                worldLayer = 19;
                viewLayer = 15;
                break;
            case 3:
                worldLayer = 20;
                viewLayer = 16;
                break;
        }
        cam.cullingMask |= 1 << viewLayer;
        cam.cullingMask &= ~(1 << worldLayer);
        ChangeLayerRecursive(worldModel, worldLayer);
        ChangeLayerRecursive(viewModel, viewLayer);
    }

    private void ChangeLayerRecursive(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursive(child, newLayer);
        }
    }
}
