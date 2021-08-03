using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private bool randomSpawns = false;
    [SerializeField] private GameObject spawnPointsRoot;
    [SerializeField] private int maxScore = 25;
    private List<PlayerController> players;
    private List<int> scores;
    private List<Transform> spawnPoints;

    private void Awake()
    {
        players = new List<PlayerController>();
        scores = new List<int>();
        if (ConnectedDevices.SharedInstance != null)
        {
            PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
            foreach (InputDevice[] player in ConnectedDevices.SharedInstance.devices)
            {
                playerInputManager.JoinPlayer(pairWithDevices: player);
            }
        }

        if (randomSpawns == true)
        {
            spawnPoints = new List<Transform>();
            foreach (Transform child in spawnPointsRoot.transform)
            {
                spawnPoints.Add(child);
            }
        }
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
            scores.Add(0);
            VirtualAudioPlayer.SharedInstance.listeners.Add(playerController.transform);
            foreach (PlayerController p in players)
            {
                p.UpdateCanvasScale();
                p.UpdateScoreText($"0 / {maxScore}");
            }

            if (randomSpawns == true)
            {
                playerController.GetComponent<CharacterController>().enabled = false;
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
                playerController.transform.position = point.position;
                playerController.transform.rotation = point.rotation;
                playerController.GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    private void OnPlayerDie(int deadIndex, int killerIndex)
    {
        if (deadIndex != killerIndex)
        {
            scores[killerIndex] += 1;
            players[killerIndex].UpdateScoreText($"{scores[killerIndex]} / {maxScore}");
        }
        print($"Player #{deadIndex} has been slain by Player #{killerIndex}");
        players[deadIndex].Disable();
        StartCoroutine(Respawn(deadIndex));
    }

    private IEnumerator Respawn(int index)
    {
        yield return new WaitForSeconds(3f);
        players[index].gameObject.SetActive(true);
        if (randomSpawns == true)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
            players[index].Respawn(point.position, point.rotation.eulerAngles);
        }
        else
        {
            players[index].Respawn(new Vector3(0f, 50f, 0f), Vector3.zero);
        }
    }
}
