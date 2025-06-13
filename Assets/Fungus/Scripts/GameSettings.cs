using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Menu : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI autoText, skipText;
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ToggleLog()
    {
        NarrativeLogMenu.NarrativeLogActive = !NarrativeLogMenu.NarrativeLogActive;
    }
    public void ToggleAuto()
    {
        SettingsData.Instance.autoForward = !SettingsData.Instance.autoForward;
        SettingsData.Save();
        if (SettingsData.Instance.autoForward)
        {
            autoText.color = Color.red;
            autoText.fontStyle = TMPro.FontStyles.Bold;
        }
        else
        {
            autoText.color = Color.white;
            autoText.fontStyle = TMPro.FontStyles.Normal;
        }
    }
    public void ToggleSkip()
    {
        SettingsData.Instance.skip = !SettingsData.Instance.skip;
        SettingsData.Save();
        if (SettingsData.Instance.skip)
        {
            skipText.color = Color.red;
            skipText.fontStyle = TMPro.FontStyles.Bold;
        }
        else
        {
            skipText.color = Color.white;
            skipText.fontStyle = TMPro.FontStyles.Normal;
        }
        Time.timeScale = SettingsData.Instance.skip ? 10f : 1f;
    }
}