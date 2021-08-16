﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Button[] defaultButtons;
    [SerializeField] private int currentPanel;

    private void Awake()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        panels[currentPanel].SetActive(true);
    }

    public void SwitchPanel(int nextPanel)
    {
        panels[currentPanel].SetActive(false);
        panels[nextPanel].SetActive(true);
        currentPanel = nextPanel;

        if (defaultButtons[currentPanel] != null)
        {
            defaultButtons[currentPanel].Select();
        }
    }

    public void LoadReadyingScene()
    {
        SceneManager.LoadScene("ReadyScreen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}