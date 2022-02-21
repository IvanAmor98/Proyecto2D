using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public GameObject settings;
    public GameObject score;
    public Text scoreText;
    public Slider soundSlider;
    public Slider playerSlider;
    public Text sliderText;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("SoundLevel"))
        {
            PlayerPrefs.SetInt("SoundLevel", 100);
        }
    }

    public void PlayClick()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingsCLick()
    {
        playerSlider.value = PlayerPrefs.GetInt("Players");
        soundSlider.value = PlayerPrefs.GetInt("SoundLevel");
        SoundSliderChanged();

        menu.SetActive(false);
        settings.SetActive(true);
    }

    public void SaveCLick()
    {
        PlayerPrefs.SetInt("Players", (int) playerSlider.value);
        PlayerPrefs.SetInt("SoundLevel", (int) soundSlider.value);
        PlayerPrefs.Save();

        AudioListener.volume = PlayerPrefs.GetInt("SoundLevel") / 100;
        menu.SetActive(true);
        settings.SetActive(false);
    }

    public void CancelCLick()
    {
        menu.SetActive(true);
        settings.SetActive(false);
    }

    public void ScoreClick()
    {
        menu.SetActive(false);
        score.SetActive(true);
        scoreText.text = GetScoreFormatted();
    }

    private string GetScoreFormatted()
    {
        string temp = PlayerPrefs.GetInt("MaxScore").ToString();
        for (int i = temp.Length; i < 6; i++)
        {
            temp = "0" + temp;
        }
        return temp;
    }

    public void ReturnClick()
    {
        menu.SetActive(true);
        score.SetActive(false);
    }

    public void QuitClick()
    {
        Application.Quit();
    }

    public void SoundSliderChanged()
    {
        sliderText.text = soundSlider.value.ToString();
    }
}
