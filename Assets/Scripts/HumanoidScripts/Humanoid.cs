using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The main mono behaviour for the humanoid
 */
public class Humanoid : MonoBehaviour
{
    private const String PROJECTILE_TAG = "Projectile";
    private const int SPAWN_FLASHES = 3;
    private const float SECONDS_BETWEEN_FLASHES = 0.3f;
    
    // Has the humanoid spawned completley
    public bool finishedSpawning { get; private set; }
    
    public delegate void OnHumanoidDeath();
    public static OnHumanoidDeath onHumanoidDeath;
    
    private HumanoidShooting _humanoidShooting;
    private HumanoidMovement _humanoidMovement;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        GameManager.Instance.humanoid = this;
    }

    private void Start()
    {
        _humanoidShooting = GetComponent<HumanoidShooting>();
        _humanoidMovement = GetComponent<HumanoidMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        finishedSpawning = false;
        StartCoroutine(Spawn());
    }
    
    private IEnumerator Spawn()
    {
        _humanoidMovement.enabled = false;
        _humanoidShooting.enabled = false;

        for (int i = 0; i < SPAWN_FLASHES; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(SECONDS_BETWEEN_FLASHES);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(SECONDS_BETWEEN_FLASHES);
        }
        _spriteRenderer.enabled = true;
        yield return new WaitForSeconds(SECONDS_BETWEEN_FLASHES);
        _humanoidMovement.enabled = true;
        _humanoidShooting.enabled = true;
        finishedSpawning = true;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(PROJECTILE_TAG) &&
            col.gameObject.GetComponent<Projectile>().getShooter() == gameObject) return;

            StartCoroutine(HumanoidDeath());
    }
    
    private IEnumerator HumanoidDeath()
    {
        _humanoidMovement.enabled = false;
        _humanoidShooting.enabled = false;
        
        float deathAnimationSeconds = GetComponent<HumanoidAnimationController>().PlayDeathAnimation();

        AudioManager.instance.PlaySound(AudioManager.HUMAN_SCREAM);
        AudioManager.instance.PlaySound(AudioManager.ELECTROCUTION_SOUND);
        
        yield return new WaitForSeconds(deathAnimationSeconds);

        AudioManager.instance.StopSound(AudioManager.HUMAN_SCREAM);
        AudioManager.instance.StopSound(AudioManager.ELECTROCUTION_SOUND);

        if (onHumanoidDeath != null)
            onHumanoidDeath();
    }
}
