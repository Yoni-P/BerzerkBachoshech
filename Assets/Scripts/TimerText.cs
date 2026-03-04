using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * The text for the timer on boss levels
 */
public class TimerText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (RobotMaker.Instance != null)
            _text.text = Mathf.RoundToInt(RobotMaker.Instance.timer).ToString();
    }
}
