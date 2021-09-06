using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectedDevices : MonoBehaviour
{
    public static ConnectedDevices SharedInstance;
    public List<InputDevice[]> devices;

    private void Awake()
    {
        if (SharedInstance != null)
        {
            Destroy(SharedInstance.gameObject);
        }
        SharedInstance = this;
        devices = new List<InputDevice[]>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        devices.Add(input.devices.ToArray());
    }
}
