using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettingsManager : MonoBehaviour
{
    [Header("⚙️ UI Elements")]
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    [SerializeField]
    private TMP_Dropdown fullscreenDropdown;

    [SerializeField]
    private TMP_Dropdown fpsDropdown;

    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    [SerializeField]
    private Button applyButton;

    [SerializeField]
    private Button cancelButton;

    private Resolution[] resolutions;

    private int savedResolution;
    private int savedFullscreenMode;
    private int savedFPS;
    private int savedQuality;

    void Start()
    {
        LoadResolutions();
        LoadQualityLevels();
        LoadSettings();

        applyButton.onClick.AddListener(ApplyChanges);
        cancelButton.onClick.AddListener(LoadSettings);
    }

    // -----------------------------------------------------------
    // Load all resolutions
    // -----------------------------------------------------------
    void LoadResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        Array.Reverse(resolutions);
        List<string> options = new List<string>();

        foreach (var r in resolutions)
            options.Add(r.width + " x " + r.height);

        resolutionDropdown.AddOptions(options);
    }

    // -----------------------------------------------------------
    // Load quality levels
    // -----------------------------------------------------------
    void LoadQualityLevels()
    {
        qualityDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var q in QualitySettings.names)
            options.Add(q);

        qualityDropdown.AddOptions(options);
    }

    // -----------------------------------------------------------
    // Save current settings
    // -----------------------------------------------------------
    void LoadSettings()
    {
        savedResolution = PlayerPrefs.GetInt("resolution", resolutions.Length - 1);
        savedFullscreenMode = PlayerPrefs.GetInt("fullscreen", 1);
        savedFPS = PlayerPrefs.GetInt("fps", 1);
        savedQuality = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());

        resolutionDropdown.value = savedResolution;
        fullscreenDropdown.value = savedFullscreenMode;
        fpsDropdown.value = savedFPS;
        qualityDropdown.value = savedQuality;

        resolutionDropdown.RefreshShownValue();
        fullscreenDropdown.RefreshShownValue();
        fpsDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();

        ApplySettingsImmediate(); // Re-apply settings without pressing Apply
    }

    // -----------------------------------------------------------
    // Apply settings immediately (internal)
    // -----------------------------------------------------------
    void ApplySettingsImmediate()
    {
        // Resolution
        Resolution res = resolutions[savedResolution];
        Screen.SetResolution(res.width, res.height, (FullScreenMode)savedFullscreenMode);

        // FPS
        int[] fpsList = { 30, 60, 120, 144, 165, 240, -1 };
        Application.targetFrameRate = fpsList[savedFPS];

        // Quality
        QualitySettings.SetQualityLevel(savedQuality);
    }

    // -----------------------------------------------------------
    // Apply settings from UI (when pressing Apply)
    // -----------------------------------------------------------
    public void ApplyChanges()
    {
        savedResolution = resolutionDropdown.value;
        savedFullscreenMode = fullscreenDropdown.value;
        savedFPS = fpsDropdown.value;
        savedQuality = qualityDropdown.value;

        // Resolution
        Resolution res = resolutions[savedResolution];
        Screen.SetResolution(res.width, res.height, (FullScreenMode)savedFullscreenMode);

        // FPS
        int[] fpsList = { 30, 60, 120, 144, 165, 240, -1 };
        Application.targetFrameRate = fpsList[savedFPS];

        // Quality
        QualitySettings.SetQualityLevel(savedQuality);

        // Save to PlayerPrefs
        PlayerPrefs.SetInt("resolution", savedResolution);
        PlayerPrefs.SetInt("fullscreen", savedFullscreenMode);
        PlayerPrefs.SetInt("fps", savedFPS);
        PlayerPrefs.SetInt("quality", savedQuality);

        PlayerPrefs.Save();
    }
}
