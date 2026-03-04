using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/**
 * Creates a flash effect from firing a laser
 */
public class ShotFlash : MonoBehaviour
{
   private const float LIFETIME = 0.3f;
   
   [SerializeField] private float _flashGrowthFactor;
   
   private Light2D _flash;
   private IEnumerator Start()
   {
      transform.SetParent(null);
      _flash = GetComponent<Light2D>();
      yield return new WaitForSeconds(LIFETIME);
      Destroy(gameObject);
   }

   private void Update()
   {
      _flash.pointLightOuterRadius *= _flashGrowthFactor;
   }
}
