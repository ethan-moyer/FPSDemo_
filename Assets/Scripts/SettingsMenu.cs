﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown presetDropdown = null;
    [SerializeField] private TMP_Dropdown resolutionsDropdown = null;
    [SerializeField] private TMP_Dropdown displayDropdown = null;
    private Resolution[] resolutions;

    private void Awake()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = $"{resolutions[i].width}x{resolutions[i].height}";
            resolutionStrings.Add(resolutionString);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(resolutionStrings);

        if (PlayerPrefs.HasKey("Resolution"))
        {
            int index = PlayerPrefs.GetInt("Resolution");
            OnChangeResolution(index);
            resolutionsDropdown.value = index;
        }
        else
        {
            resolutionsDropdown.value = currentResolutionIndex;
        }

        if (PlayerPrefs.HasKey("QualityLevel"))
        {

            int level = PlayerPrefs.GetInt("QualityLevel");
            OnChangePreset(level);
            presetDropdown.value = level;
        }

        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            OnChangeDisplayMode(PlayerPrefs.GetInt("DisplayMode"));
        }
    }

    public void OnChangePreset(int preset)
    {
        if (preset != QualitySettings.GetQualityLevel())
        {
            QualitySettings.SetQualityLevel(preset);
            PlayerPrefs.SetInt("QualityLevel", preset);
        }
    }

    public void OnChangeResolution(int resolution)
    {
        Screen.SetResolution(resolutions[resolution].width, resolutions[resolution].height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", resolution);
    }

    public void OnChangeDisplayMode(int mode)
    {
        if (mode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (mode == 1)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        PlayerPrefs.SetInt("DisplayMode", mode);
    }
}
