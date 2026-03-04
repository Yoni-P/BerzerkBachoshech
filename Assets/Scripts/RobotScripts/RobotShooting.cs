using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the shooting of the robots
 */
public class RobotShooting : MonoBehaviour
{
    private const string TARGETS_LAYER = "Targets";
    
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _robotReloadTime;

    private bool _canShoot = true;
    private static Vector2[] _shootingDirections =  {
        Vector2.down, Vector2.left, Vector2.up, Vector2.right, Vector2.up+Vector2.right, 
        Vector2.up+Vector2.left, Vector2.down+Vector2.right, Vector2.down+Vector2.left
    };
    private Humanoid _humanoid;
    private BoxCollider2D _boxCollider2D;
    private Color _robotColor;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _robotColor = GetComponent<SpriteRenderer>().color;
    }

    private void Update()
    {
        foreach (var direction in _shootingDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, int.MaxValue, 
                LayerMask.GetMask(TARGETS_LAYER));
            if (hit && _canShoot)
            {
                Shoot(direction);
                break;
            }
        }
    }

    private void Shoot(Vector2 shootDirection)
    {
        Vector2 robotPosition = transform.position;
        
        Vector2 projectileDirection = shootDirection;

        var robotColliderBounds = _boxCollider2D.bounds;
        Vector3 projectilePosition = 
            new Vector3(robotPosition.x + robotColliderBounds.extents.x * projectileDirection.x,
            robotPosition.y + robotColliderBounds.extents.y * projectileDirection.y, 0);
        
        var newProjectile = Instantiate(_projectile.gameObject, projectilePosition, 
            Quaternion.identity);
        
        newProjectile.GetComponent<Projectile>().Setup(projectileDirection, _projectileSpeed, gameObject, _robotColor);
        
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        _canShoot = false;

        yield return new WaitForSeconds(_robotReloadTime);
        
        _canShoot = true;
    }
}
