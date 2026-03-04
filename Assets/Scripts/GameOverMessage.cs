using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Shows the game over message 
 */
public class GameOverMessage : MonoBehaviour
{

    private void Start()
    {
        String scoreText = (GameManager.points / 1000 % 10).ToString() + (GameManager.points / 100 % 10).ToString() +
                          (GameManager.points / 10 % 10).ToString() +
                          (GameManager.points % 10).ToString();
        GetComponent<TextMeshProUGUI>().text = "Your Score: " + scoreText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadSceneAsync("Opening Screen");
    }
}
