using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the shooting behaviour of the humanoid
 */
public class HumanoidShooting : MonoBehaviour
{
    private const String PLAYER_SHOTS_LAYER = "Player Shots";
    
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _shootDirectionInputDelay;
    [SerializeField] private float _afterShootDelay;
    [SerializeField] private float _reloadTime;
    
    private PlayerInput _playerInput;
    private HumanoidAnimationController _humanoidAnimation;
    private bool _canShoot = true;
    private Color _humanoidColor = new Color(0, 230, 0);

    // Is the humanoid in the middle of shooting a projectile
    public bool isShooting { get; private set; }
    

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _humanoidAnimation = GetComponent<HumanoidAnimationController>();
    }

    private void Update()
    {
        if (_playerInput.shootButtonPressed)
            StartCoroutine(Shoot());
    }
    
    private IEnumerator Shoot()
    {
        if (!_canShoot || _playerInput.arrowKeysInput == Vector2Int.zero) yield break;
        
        isShooting = true;
        _canShoot = false;
        
        yield return new WaitForSeconds(_shootDirectionInputDelay);
        
        Vector2 projectileDirection;
        
        do
        {
            projectileDirection = _playerInput.arrowKeysInput;
            yield return null;
            if (!_playerInput.shootButtonPressed)
            {
                _canShoot = true;
                isShooting = false;
                yield break;
            }
        } while (projectileDirection == Vector2.zero);
        
        _humanoidAnimation.ChangeShootingAnimation(projectileDirection);

        var projectileSpawn = GetProjectileSpawn(projectileDirection);
        var newProjectile = Instantiate(_projectile.gameObject, projectileSpawn, 
            Quaternion.identity);
        
        newProjectile.GetComponent<Projectile>().Setup(projectileDirection, _projectileSpeed,
            gameObject,_humanoidColor, LayerMask.NameToLayer(PLAYER_SHOTS_LAYER));
        
        yield return new WaitForSeconds(_afterShootDelay);
        isShooting = false;
        
        yield return new WaitForSeconds(_reloadTime);
        _canShoot = true;
    }
    
    private Vector3 GetProjectileSpawn(Vector2 projectileDirection)
    {
        // TODO: Put all the projectile spawn points in variables
        if (projectileDirection == Vector2.up)
            return transform.TransformPoint(new Vector3(0.63f, 1.61f));
        if (projectileDirection == Vector2.up + Vector2.right)
            return transform.TransformPoint(new Vector3(0.98f, 1.98f));
        if (projectileDirection == Vector2.right)
            return transform.TransformPoint(new Vector3(1.1f, 1f));
        if (projectileDirection == Vector2.right + Vector2.down)
            return transform.TransformPoint(new Vector3(0.85f, -0.22f));
        if (projectileDirection == Vector2.down)
            return transform.TransformPoint(new Vector3(0.5f, -0.6f));
        if (projectileDirection == Vector2.down + Vector2.left)
            return transform.TransformPoint(new Vector3(-1.1f, -0.21f));
        if (projectileDirection == Vector2.left)
            return transform.TransformPoint(new Vector3(-1.1f, 1f));
        if (projectileDirection == Vector2.left + Vector2.up)
            return transform.TransformPoint(new Vector3(-0.97f, 2.22f));
        return Vector3.zero;
    }

    private void OnDisable()
    {
        _canShoot = false;
    }

    private void OnEnable()
    {
        _canShoot = true;
    }
}
