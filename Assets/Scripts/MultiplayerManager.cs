using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager SharedInstance;
    [SerializeField] private bool randomSpawns = false;
    [SerializeField] private GameObject spawnPointsRoot;
    [SerializeField] private int maxScore = 25;
    [SerializeField] private GameObject previewCam;
    [SerializeField] private GameObject previewButtons;
    [SerializeField] private TextMeshProUGUI previewText;
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject pauseCanvas;
    private PlayerInputManager playerInputManager;
    private List<PlayerController> players;
    private List<int> scores;
    private List<Transform> spawnPoints;
    private bool paused;

    private void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
        else
            Destroy(gameObject);

        players = new List<PlayerController>();
        scores = new List<int>();
        playerInputManager = GetComponent<PlayerInputManager>();
        if (ConnectedDevices.SharedInstance != null)
        {
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
            StartCoroutine(StartRound());
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

    private IEnumerator StartRound()
    {
        previewCam.SetActive(true);
        previewButtons.SetActive(false);
        blackScreen.CrossFadeAlpha(0f, 0.2f, false);
        previewText.text = $"First to {maxScore} Wins";
        yield return new WaitForSeconds(2f);
        blackScreen.canvasRenderer.SetAlpha(1f);
        yield return new WaitForEndOfFrame();
        previewCam.SetActive(false);
        foreach (InputDevice[] player in ConnectedDevices.SharedInstance.devices)
        {
            playerInputManager.JoinPlayer(pairWithDevices: player);
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
                p.UpdateCanvasScale(players.Count);
                p.UpdateScoreText($"0 / {maxScore}");
            }

            if (randomSpawns == true)
            {
                playerController.GetComponent<CharacterController>().enabled = false;
                Transform point = FindSpawnPoint();
                playerController.transform.position = point.position;
                playerController.transform.rotation = point.rotation;
                playerController.GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    public void OnRematchClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitClicked()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnPausePressed()
    {
        if (!paused)
        {
            pauseCanvas.SetActive(true);
            EventSystem ev = pauseCanvas.GetComponent<EventSystem>();
            ev.SetSelectedGameObject(null);
            ev.SetSelectedGameObject(ev.firstSelectedGameObject);
            foreach (PlayerController player in players)
            {
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            }
            Cursor.lockState = CursorLockMode.None;
            paused = true;
        }
        else
        {
            pauseCanvas.SetActive(false);
            foreach (PlayerController player in players)
            {
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            }
            Cursor.lockState = CursorLockMode.Locked;
            paused = false;
        }
    }

    private void OnPlayerDie(int deadIndex, int killerIndex)
    {
        if (deadIndex != killerIndex)
        {
            scores[killerIndex] += 1;
            players[killerIndex].UpdateScoreText($"{scores[killerIndex]} / {maxScore}");
            if (scores[killerIndex] >= maxScore)
            {
                EndGame(killerIndex);
                return;
            }
        }
        print($"Player #{deadIndex} has been slain by Player #{killerIndex}");
        players[deadIndex].Disable();
        StartCoroutine(Respawn(deadIndex));
    }

    private void EndGame(int winner)
    {
        foreach (PlayerController player in players)
        {
            player.gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        previewCam.SetActive(true);
        blackScreen.gameObject.SetActive(false);
        previewButtons.SetActive(true);
        previewText.text = $"Player {winner + 1} Wins!";
    }

    private IEnumerator Respawn(int index)
    {
        yield return new WaitForSeconds(3f);
        players[index].gameObject.SetActive(true);
        if (randomSpawns == true)
        {
            Transform point = FindSpawnPoint();
            players[index].Respawn(point.position, point.rotation.eulerAngles);
        }
        else
        {
            players[index].Respawn(new Vector3(0f, 50f, 0f), Vector3.zero);
        }
    }

    private Transform FindSpawnPoint()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
        bool validPoint = true;

        foreach (PlayerController player in players)
        {
            if (Vector3.Distance(player.transform.position, point.position) <= 5f)
            {
                validPoint = false;
                break;
            }
        }

        if (validPoint)
            return point;
        else
            return FindSpawnPoint();
    }
}
