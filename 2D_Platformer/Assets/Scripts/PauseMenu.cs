using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject videoOptions;
    public GameObject audioOptions;

    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        videoOptions.SetActive(false);
        audioOptions.SetActive(false);
        optionMenu.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
    public void Options()
    {
        pauseMenu.SetActive(false);
        optionMenu.SetActive(true);
    }
    public void OptionsBack()
    {
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
    }
    public void VideoMenu()
    {
        videoOptions.SetActive(true);
        optionMenu.SetActive(false);
    }
    public void VideoMenuBack()
    {
        videoOptions.SetActive(false);
        optionMenu.SetActive(true);
    }
    public void AudioMenu()
    {
        audioOptions.SetActive(true);
        optionMenu.SetActive(false);
    }
    public void AudioMenuBack()
    {
        audioOptions.SetActive(false);
        optionMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
