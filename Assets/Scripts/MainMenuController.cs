using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * A controller for the main menu
 */
public class MainMenuController : MonoBehaviour
{
    private const String FIRE_BUTTON = "Fire1";
    private const String GAME_LEVEL_SCENE = "Level";
    private void Update()
    {
        if ((int)Input.GetAxisRaw(FIRE_BUTTON) == 1)
        {
            GameManager.ResetGameStats();
            SceneManager.LoadScene(GAME_LEVEL_SCENE);
        }
    }
}
