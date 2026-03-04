using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = System.Random;

public class LightningEffect : MonoBehaviour
{
    [SerializeField] private int _minSecondsDelay, _maxSecondsDelay;
    [SerializeField] private int _minNumFlashes, _maxNumFlashes;
    [SerializeField] private float _singleFlashDuration;
    
    private Random _random;
    private Light2D _light2D;
    

    private void Start()
    {
        _light2D = GetComponent<Light2D>();
        _random = new Random();
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        int numberOfFlashes = _random.Next(_minNumFlashes, _maxNumFlashes);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            _light2D.intensity = 1;
            yield return new WaitForSeconds(_singleFlashDuration);
            _light2D.intensity = 0;
            yield return new WaitForSeconds(_singleFlashDuration);
        }

        yield return new WaitForSeconds(_random.Next(_minSecondsDelay, _maxSecondsDelay));

        StartCoroutine(Flash());
    }
}
