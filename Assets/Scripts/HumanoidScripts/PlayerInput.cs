using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Read input from the player for controlling the humanoid
 */
public class PlayerInput : MonoBehaviour
{
    // The direction input
    public Vector2Int arrowKeysInput { get; private set; }
    // Shooting button input
    public bool shootButtonPressed { get; private set; }

    private void Update()
    {
        int xInput = (int)Mathf.Round(Input.GetAxisRaw("Horizontal"));
        int yInput = (int)Mathf.Round(Input.GetAxisRaw("Vertical"));

        arrowKeysInput = new Vector2Int(xInput, yInput);

        shootButtonPressed = (int)Input.GetAxisRaw("Fire1") == 1;
    }
}
