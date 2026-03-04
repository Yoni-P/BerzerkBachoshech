using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controld the robots movement
 */
public class RobotMovement : MonoBehaviour
{
    private const String WALLS_LAYER = "Obstacles";
    private const String WALKING_UP = "Walking Up";
    private const String WALKING_DOWN = "Walking Down";
    private const String WALKING_SIDEWAYS = "Walking Sideways";
    private const String WALKING_SPEED = "Walking Speed";
    private const int UP = 1;
    private const int RIGHT = 1;
    private const int DOWN = -1;
    private const int LEFT = -1;
    
    [SerializeField] private float _robotSpeed;
    [SerializeField] private Animator _animator;
    
    private Grid _mazeCellGrid;
    private float _minVerticalDistanceFromWall;
    private float _minHorizontalDistanceFromWall;
    private bool _canGoUp, _canGoDown, _canGoLeft, _canGoRight;
    private PixelatedMovement _pixelatedMovement;
    private Humanoid _humanoid;
    
    private void Start()
    {
        _humanoid = GameManager.Instance.humanoid;
        _mazeCellGrid = GameManager.Instance.MazeCellGrid;
        _pixelatedMovement = GetComponent<PixelatedMovement>();
        var cellSize = _mazeCellGrid.cellSize;
        _minVerticalDistanceFromWall = cellSize.y * 2 / 3;
        _minHorizontalDistanceFromWall = cellSize.x * 2 / 3;
    }

    private void Update()
    {
        SetDirectionRestrictions();
        SetNewMovementDirection();
    }

    private void SetNewMovementDirection()
    {
        Vector3 robotPosition = transform.position;
        Vector3 humanoidPosition = _humanoid.transform.position;
        
        Vector2 movementDirection = Vector2.zero;

        if (Mathf.Abs(humanoidPosition.y - robotPosition.y) > 0.2)
        {
            if (humanoidPosition.y < robotPosition.y && _canGoDown)
                movementDirection.y = -1;
            else if (humanoidPosition.y > robotPosition.y && _canGoUp)
                movementDirection.y = 1;
        }

        if (Mathf.Abs(humanoidPosition.x - robotPosition.x) > 0.2)
        {
            if (humanoidPosition.x < robotPosition.x && _canGoLeft)
                movementDirection.x = -1;
            else if (humanoidPosition.x > robotPosition.x && _canGoRight)
                movementDirection.x = 1;
        }

        UpdateAnimations(movementDirection);
        _pixelatedMovement.SetVelocity(movementDirection * _robotSpeed);
    }

    private void UpdateAnimations(Vector2 movementDirection)
    {
        float xScale = Mathf.Approximately(movementDirection.x, -1) ? -1 : 1;
        
        transform.localScale = new Vector3(xScale, 1, 0);
        
        _animator.SetBool(WALKING_SIDEWAYS, !Mathf.Approximately(movementDirection.x, 0));
        _animator.SetBool(WALKING_UP, Mathf.Approximately(movementDirection.y, 1));
        _animator.SetBool(WALKING_DOWN, Mathf.Approximately(movementDirection.y, -1));
        _animator.SetFloat(WALKING_SPEED, _robotSpeed);
    }

    private void SetDirectionRestrictions()
    {
        int obstaclesLayerMask = LayerMask.GetMask(WALLS_LAYER);
        
        Vector3 robotPosition = transform.position;
        Vector3Int humanoidGridLocation = _mazeCellGrid.WorldToCell(_humanoid.transform.position);
        Vector3Int robotGridLocation = _mazeCellGrid.WorldToCell(robotPosition);

        bool humanInSameCellAsRobot = humanoidGridLocation == robotGridLocation;
        
        _canGoDown = humanInSameCellAsRobot || !Physics2D.Raycast(robotPosition, Vector2.down,
            _minVerticalDistanceFromWall, obstaclesLayerMask);
        _canGoUp = humanInSameCellAsRobot || !Physics2D.Raycast(robotPosition, Vector2.up,
            _minVerticalDistanceFromWall, obstaclesLayerMask);
        _canGoRight = humanInSameCellAsRobot || !Physics2D.Raycast(robotPosition, Vector2.right,
            _minHorizontalDistanceFromWall, obstaclesLayerMask);
        _canGoLeft = humanInSameCellAsRobot || !Physics2D.Raycast(robotPosition, Vector2.left,
            _minHorizontalDistanceFromWall, obstaclesLayerMask);
    }
}
