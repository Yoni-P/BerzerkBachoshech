using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/**
 * Controls the laser projectiles
 */
public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D;
    
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private Light2D _shotFlash;
    
    private GameObject _shooter;
    private PixelatedMovement _movement;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movement = GetComponent<PixelatedMovement>();
    }
    
    private void Start()
    {
        AudioManager.instance.PlaySound(AudioManager.LASER_SHOT);
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    /**
     * Setup for the projectile
     */
    public void Setup(Vector2 direction, float speed, GameObject shooter, Color color, int layer = 0)
    {
        _spriteRenderer.color = color;
        _shotFlash.color = color;
        _movement.MoveToPixelGrid();
        gameObject.layer = layer;
        _shooter = shooter;
        Vector2 newVelocity = direction.normalized * speed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        _movement.SetVelocity(newVelocity);
    }

    private IEnumerator OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject != _shooter)
        {
            GameObject hitEffect = Instantiate(_hitEffect.gameObject, transform.position, Quaternion.identity);
            _rigidBody2D.simulated = false;
            _spriteRenderer.enabled = false;
            GetComponent<Light2D>().enabled = false;
            yield return new WaitForSeconds(1);
            Destroy(hitEffect);
            Destroy(gameObject);
        }
    }

    /**
     * Return the object that "shot" the projectile
     */
    public GameObject getShooter()
    {
        return _shooter;
    }
}
