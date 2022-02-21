using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject[] levels;
    public GameObject gameOverText;
    public GameObject gameCompleteText;
    public GameObject gameCompletePanel;
    public GameObject newHighScorePanel;
    public GameObject firstPlayer;
    public GameObject playerPrefab;
    public List<GameObject> bricks;
    private AudioSource audioSource;
    private int score;
    private int balls = 3;
    public Text scoreText;
    public Text finalScoreText;
    public Text ballsText;
    private bool record = false;

    void Start()
    {
        Time.timeScale = 1;
        if (PlayerPrefs.HasKey("SoundLevel"))
        {
            AudioListener.volume = PlayerPrefs.GetInt("SoundLevel") / 100F;
        }
        if (PlayerPrefs.GetInt("Players") == 1)
        {
            firstPlayer.transform.position = new Vector3(4, firstPlayer.transform.position.y, firstPlayer.transform.position.z);
            GameObject tempPlayer = Instantiate(playerPrefab, new Vector3(-4, firstPlayer.transform.position.y, firstPlayer.transform.position.z), Quaternion.identity);
            tempPlayer.SendMessage("SetSecondPlayer");
        }
        audioSource = GetComponent<AudioSource>();
        score = GameStage.score;
        balls = GameStage.balls + 4;
        Instantiate(levels[GameStage.gameStage - 1], new Vector3(0, 0, -5), Quaternion.identity);
        bricks = GameObject.FindGameObjectsWithTag("brick").ToList();
        UpdateCanvas();
    }

    public void PlayBrickSound()
    {
        audioSource.Play();
    }

    public void DeductBall(GameObject ball)
    {
        if (balls == 0)
        {
            Destroy(ball);
            GameOver();
        }
        balls--;
        UpdateCanvas();
    }

    public void AddScore(int quantity)
    {
        score += quantity;
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        ballsText.text = balls.ToString();
        scoreText.text = GetScoreFormatted();
    }

    private string GetScoreFormatted()
    {
        string temp = score.ToString();
        for (int i = temp.Length; i < 6; i++)
        {
            temp = "0" + temp;
        }
        return temp;
    }

    private void BrickDeath(GameObject gameObject)
    {
        bricks.Remove(gameObject);
        if (bricks.Count == 0) ScreenClear();
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        if (PlayerPrefs.GetInt("MaxScore") < score)
        {
            PlayerPrefs.SetInt("MaxScore", score);
            record = true;
        }
        gameCompletePanel.SetActive(true);
        gameOverText.SetActive(true);
        gameCompleteText.SetActive(false);
        newHighScorePanel.SetActive(record);
        finalScoreText.text = GetScoreFormatted();

        ballsText.GetComponentInParent<GameObject>().SetActive(false);
        scoreText.GetComponentInParent<GameObject>().SetActive(false);

        GameStage.score = 0;
        GameStage.balls = 3;
        GameStage.gameStage = 1;
        Time.timeScale = 0;
    }

    private void ScreenClear()
    {
        GameStage.gameStage++;
        GameStage.score = score;
        GameStage.balls = balls;
        if (GameStage.gameStage > 2)
        {
            Time.timeScale = 0;
            if (PlayerPrefs.GetInt("MaxScore") < score)
            {
                PlayerPrefs.SetInt("MaxScore", score);
                record = true;
            }
            gameCompletePanel.SetActive(true);
            gameOverText.SetActive(false);
            gameCompleteText.SetActive(true);
            newHighScorePanel.SetActive(record);
            finalScoreText.text = GetScoreFormatted();

            GameStage.score = 0;
            GameStage.balls = -1;
            GameStage.gameStage = 1;

            ballsText.GetComponentInParent<GameObject>().SetActive(false);
            scoreText.GetComponentInParent<GameObject>().SetActive(false);
        }
        else 
        {
            SceneManager.LoadScene(1);
        }
    }

    public void PlayAgainClick()
    {
        GameStage.gameStage = 1;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    
    public void MainMenuClick()
    {
        GameStage.gameStage = 1;
        SceneManager.LoadScene(0);
    }
}
