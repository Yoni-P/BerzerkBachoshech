using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * Can be used to make text flash in a consistent manner
 */
public class CyclicTextFlashing : MonoBehaviour
{
    private const float DISSAPEAR_SECONDS = 1;
    private const float APPEAR_SECONDS = 1;
    
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        _text.enabled = true;
        yield return new WaitForSeconds(APPEAR_SECONDS);
        _text.enabled = false;
        yield return new WaitForSeconds(DISSAPEAR_SECONDS);
        StartCoroutine(Flash());
    }
}
