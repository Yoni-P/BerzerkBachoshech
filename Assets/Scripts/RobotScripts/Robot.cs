using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/**
 * The main robot behaviour
 */
public class Robot : MonoBehaviour
{
    private const String STARTED_MOVEMENT = "Started Movement";
    private const String ROBOT_DEATH = "Death";
    
    [SerializeField] Animator _animator;
    
    public delegate void OnRobotDeath();
    public static OnRobotDeath onRobotDeath;
    
    private bool _canShoot, _canMove;
    private RobotShooting _robotShooting;
    private RobotMovement _robotMovement;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _robotShooting = GetComponent<RobotShooting>();
        _robotMovement = GetComponent<RobotMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        _robotMovement.enabled = false;
        _robotShooting.enabled = false;
        float spawnAnimationDuration = _animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(spawnAnimationDuration);
        _animator.SetBool(STARTED_MOVEMENT, true);
        yield return new WaitForSeconds((float)new Random().Next(10, 20) / 20f);
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        _robotMovement.enabled = true;
        _robotShooting.enabled = true;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Projectile") && col.gameObject.GetComponent<Projectile>().getShooter() == gameObject)
            return;
        
        Hit();
    }

    private void Hit()
    {
        if (onRobotDeath != null)
            onRobotDeath();
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        Destroy(transform.GetChild(0).gameObject);
        _robotShooting.enabled = false;
        _robotMovement.enabled = false;
        _rigidbody2D.simulated = false;
        AudioManager.instance.PlaySound(AudioManager.ROBOT_EXPLOSION);
        _animator.Play(ROBOT_DEATH);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
        Destroy(gameObject);
    }
}
