using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown presetDropdown = null;
    [SerializeField] private TMP_Dropdown resolutionsDropdown = null;
    [SerializeField] private TMP_Dropdown displayDropdown = null;
    [SerializeField] private TMP_Dropdown physicsDropdown = null;
    private Resolution[] resolutions;

    public void LoadSettings()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
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

        if (PlayerPrefs.HasKey("PhysicsMode"))
        {
            print(PlayerPrefs.GetInt("PhysicsMode"));
            physicsDropdown.value = PlayerPrefs.GetInt("PhysicsMode");
        }
        else
        {
            PlayerPrefs.SetInt("PhysicsMode", 0);
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
        displayDropdown.value = mode;
        PlayerPrefs.SetInt("DisplayMode", mode);
    }

    public void OnChangePhysicsMode(int mode)
    {
        PlayerPrefs.SetInt("PhysicsMode", mode);
    }

    public void OnReturnToMenu()
    {
        PlayerPrefs.Save();
    }
}
