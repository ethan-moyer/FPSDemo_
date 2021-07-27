using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyScreenUIController : MonoBehaviour
{
    public string levelName = "";
    [SerializeField] private TextMeshProUGUI[] playerPanelTexts;
    [SerializeField] private Image loadingScreen;
    private bool loadingLevel;

    private void Awake()
    {
        loadingLevel = false;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.canvasRenderer.SetAlpha(0f);
    }

    public void OnStartLevel()
    {
        if (loadingLevel == false)
        {
            loadingLevel = true;
            loadingScreen.CrossFadeAlpha(1f, 0.1f, false);
            StartCoroutine(LoadLevel());
        }
    }

    private IEnumerator LoadLevel()
    {
        print("Loading level");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }
        }

        yield return null;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        playerPanelTexts[input.playerIndex].text = input.currentControlScheme;
        if (input.currentControlScheme == "Keyboard")
        {
            playerPanelTexts[input.playerIndex].text += "\nPress Enter to Begin";
        }
        else if (input.currentControlScheme == "Gamepad")
        {
            playerPanelTexts[input.playerIndex].text += "\nPress Start to Begin";
        }
    }
}
