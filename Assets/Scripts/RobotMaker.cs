using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using Random = System.Random;

/**
 * Spawns robots in the maze for a limited amount of time
 */
public class RobotMaker : MonoBehaviour
{

    [SerializeField] private int _minSecondsBetweenSpawns, _maxSecondsBetweenSpawns;
    [SerializeField] private TrailRenderer _trailEffect;
    [SerializeField] private PulsingLight pulsingLight;

    [field:SerializeField] public float timer { get; private set; }
    
    private Robot _robot;
    private int _mazeHeight, _mazeWidth;
    private Grid _cellGrid;
    private Random _random;
    private Humanoid _humanoid;
    private bool _openedGates;
    public static RobotMaker Instance;

    public delegate void OnTimerDone();

    public static OnTimerDone onTimerDone;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        _mazeHeight = GameManager.Instance.MazeHeight;
        _mazeWidth = GameManager.Instance.MazeWidth;
        _cellGrid = GameManager.Instance.MazeCellGrid;
        _humanoid = GameManager.Instance.humanoid;
        _robot = GameManager.Instance.GetRobotByDifficultyLevel();
        _random = new Random();

        yield return new WaitUntil(() => _humanoid.finishedSpawning);

        StartCoroutine(BuildRobots());
    }

    private IEnumerator BuildRobots()
    {
        yield return new WaitForSeconds(_random.Next(_minSecondsBetweenSpawns, _maxSecondsBetweenSpawns));

        Vector3Int cellToSpawnIn;
        do
        {
            cellToSpawnIn = new Vector3Int(-_random.Next(_mazeHeight), _random.Next(_mazeWidth));
        } while (cellToSpawnIn == _cellGrid.WorldToCell(transform.position) ||
                 cellToSpawnIn == _cellGrid.WorldToCell(_humanoid.transform.position));

        BoxCollider2D robotCollider = _robot.GetComponent<BoxCollider2D>();
        Vector3 cellCenter = _cellGrid.GetCellCenterWorld(cellToSpawnIn);
        Vector3 cellExtents = new Vector3(_cellGrid.cellSize.x / 4, -_cellGrid.cellSize.y / 4);
        Vector3 topLeftCorner = cellCenter - cellExtents;
        Vector3 bottomRightCorner = cellCenter + cellExtents;

        int x, y;

        do
        {
            x = _random.Next((int)topLeftCorner.x, (int)bottomRightCorner.x);
            y = _random.Next((int)bottomRightCorner.y, (int)topLeftCorner.y);
        } while (Physics.BoxCast(new Vector3(x, y), robotCollider.bounds.extents, Vector3.zero));

        StartCoroutine(SpawnRobot(new Vector3(x, y)));
        
        if (timer > 0)
            StartCoroutine(BuildRobots());
    }

    private void Update()
    {
        if (_humanoid.finishedSpawning)
        {
            if (timer > 0) timer -= Time.deltaTime;
            else if (! _openedGates)
            {
                timer = 0;
                _openedGates = true;
                pulsingLight.enabled = false;
                if (onTimerDone != null)
                {
                    Debug.Log("Called onTimerDone");
                    onTimerDone();
                }
                    
            }
        }
    }

    private IEnumerator SpawnRobot(Vector3 spawnPoint)
    {
        _trailEffect.enabled = true;
        _trailEffect.transform.position = spawnPoint;
        
        yield return null;
        
        Instantiate(_robot.gameObject, spawnPoint, Quaternion.identity);

        yield return new WaitForSeconds(0.1f);
        _trailEffect.enabled = false;
        _trailEffect.transform.position = transform.position;
    }
}
