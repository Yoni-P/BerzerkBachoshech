using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the animations of the human
 */
public class HumanoidAnimationController : MonoBehaviour
{
    // Animation Names
    private const String IDLE = "Idle";
    private const String WALK_LEFT = "Walk Left";
    private const String WALK_RIGHT = "Walk Right";
    private const String SHOOT_UP = "Shoot Up";
    private const String SHOOT_RIGHT_UP = "Shoot Right Up";
    private const String SHOOT_RIGHT = "Shoot Right";
    private const String SHOOT_RIGHT_DOWN = "Shoot Right Down";
    private const String SHOOT_DOWN = "Shoot Down";
    private const String SHOOT_LEFT_DOWN = "Shoot Left Down";
    private const String SHOOT_LEFT = "Shoot Left";
    private const String SHOOT_LEFT_UP = "Shoot Left Up";
    private const string DEATH = "Death";

    [SerializeField] private Animator _animator;
    
    private string _currentState;
    
    private void Start()
    {
        ChangeAnimatorState(IDLE);
    }
    
    /**
     * Changes the animation to a shooting animation based on given direction
     */
    public void ChangeShootingAnimation(Vector2 shootingDirection)
    {
        String newShootingState = IDLE;
        if (shootingDirection == Vector2.up) newShootingState = SHOOT_UP;
        else if (shootingDirection == Vector2.right + Vector2.up) newShootingState = SHOOT_RIGHT_UP;
        else if (shootingDirection == Vector2.right) newShootingState = SHOOT_RIGHT;
        else if (shootingDirection == Vector2.right + Vector2.down) newShootingState = SHOOT_RIGHT_DOWN;
        else if (shootingDirection == Vector2.down) newShootingState = SHOOT_DOWN;
        else if (shootingDirection == Vector2.left + Vector2.down) newShootingState = SHOOT_LEFT_DOWN;
        else if (shootingDirection == Vector2.left) newShootingState = SHOOT_LEFT;
        else if (shootingDirection == Vector2.left + Vector2.up) newShootingState = SHOOT_LEFT_UP;
        ChangeAnimatorState(newShootingState);
    }

    /**
     * Changes the animation to a movement animation based on a given direction
     */
    public void ChangeMovementAnimation(Vector2 movementDirection)
    {
        if (movementDirection == Vector2.zero) ChangeAnimatorState(IDLE);
        else if (Mathf.Approximately(movementDirection.x, -1)) ChangeAnimatorState(WALK_LEFT);
        else ChangeAnimatorState(WALK_RIGHT);
    }

    /**
     * Plays the humanoid death animation
     */
    public float PlayDeathAnimation()
    {
        ChangeAnimatorState(DEATH);
        return _animator.GetCurrentAnimatorClipInfo(0).Length;
    }

    private void ChangeAnimatorState(String newState)
    {
        if (newState == _currentState) return;
        
        _animator.Play(newState);

        _currentState = newState;
    }
}
