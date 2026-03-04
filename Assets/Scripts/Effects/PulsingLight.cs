using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/**
 * Pulses a light gently
 */
public class PulsingLight : MonoBehaviour
{
    private Light2D _light2D;

    private void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        _light2D.intensity = Mathf.Abs(Mathf.Sin(Time.time * 8)) / 4 + 0.5f;
    }
}
