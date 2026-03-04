using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to exit the game any time with escape
 */
public class EscapeScript : MonoBehaviour
{
    private static EscapeScript _instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
