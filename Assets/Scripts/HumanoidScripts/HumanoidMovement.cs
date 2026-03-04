using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the movement of the humanoid
 */
public class HumanoidMovement : MonoBehaviour
{
    [SerializeField] private float _playerSpeed;
    
    private PixelatedMovement _pixelatedMovement;
    private HumanoidShooting _humanoidShooting;
    private PlayerInput _playerInput;
    private HumanoidAnimationController _humanoidAnimation;

    private void Start()
    {
        _pixelatedMovement = GetComponent<PixelatedMovement>();
        _humanoidShooting = GetComponent<HumanoidShooting>();
        _playerInput = GetComponent<PlayerInput>();
        _humanoidAnimation = GetComponent<HumanoidAnimationController>();
    }

    private void Update()
    {
        if (_humanoidShooting.isShooting || _playerInput.shootButtonPressed)
        {
            _pixelatedMovement.SetVelocity(Vector2.zero);
            return;
        }

        Vector2 movementDirection = _playerInput.arrowKeysInput;
        _pixelatedMovement.SetVelocity(movementDirection * _playerSpeed);
        _humanoidAnimation.ChangeMovementAnimation(movementDirection);
    }

    private void OnDisable()
    {
        if (_pixelatedMovement != null)
            _pixelatedMovement.SetVelocity(Vector2.zero);
    }
}
