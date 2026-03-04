using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/**
 * The main game manager
 */
public class GameManager : MonoBehaviour
{
    private const int STARTING_LIVES = 3;
    private const int STARTING_LEVEL = 1;
    private const int STARTING_POINTS = 0;
    
    [field: SerializeField] public int MazeWidth { get; private set; }
    [field: SerializeField] public int MazeHeight { get; private set; }
    [field: SerializeField] public Grid MazeCellGrid { get; private set; }
    
    public int MazeCellWidth { get; private set; }
    public int MazeCellHeight { get; private set; }
    public static int points { get; private set; }
    public int difficultyLevel
    {
        get => level / _difficultyIncreaseInterval;
    }
    public static GameManager Instance { get; private set; }
    public static int level;
    public static Vector2 EnterDirection;
    public MazeGenerator mazeGenerartor;
    public Humanoid humanoid;

    [SerializeField] private int _difficultyIncreaseInterval;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject[] graphicLives;
    [SerializeField] private Robot[] _robots;
    [SerializeField] private int _pointsForKill = 100;
    [SerializeField] private Tilemap _maze;
    
    private bool _humanoidEscaped;
    private static int lives;
    
    private void Awake()
    {
        Instance = this;
        
        var cellSize = MazeCellGrid.cellSize;
        MazeCellHeight = (int)cellSize.x + 2;
        MazeCellWidth = (int)cellSize.y + 2;
    }

    private IEnumerator Start()
    {
        DisableExtraLives();
        updateScore();
        yield return new WaitUntil(() => mazeGenerartor != null);
        Robot.onRobotDeath += AddPointsForKillingRobot;
        Humanoid.onHumanoidDeath += HumanoidDied;
    }

    private void DisableExtraLives()
    {
        for (int i = lives; i < graphicLives.Length; i++)
        {
            graphicLives[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (humanoid.gameObject != null && !_humanoidEscaped)
            CheckHumanoidEscaped();
    }

    private void OnDestroy()
    {
        Humanoid.onHumanoidDeath -= HumanoidDied;
        Robot.onRobotDeath -= AddPointsForKillingRobot;
    }

    private IEnumerator HumanoidEscaped(Vector2 exitDirection)
    {
        _humanoidEscaped = true;
        EnterDirection = -exitDirection;
        level++;
        SceneManager.LoadSceneAsync("Level");
        yield break;
    }

    private void HumanoidDied()
    {
        if (lives == 0)
        {
            GameOver();
            return;
        }

        level += level % _difficultyIncreaseInterval == 0 ? 1 : 0;
        lives--;
        EnterDirection = Vector2.zero;
        SceneManager.LoadSceneAsync("Level");
    }

    private void GameOver()
    {
        SceneManager.LoadSceneAsync("Game Over");
    }

    private void AddPointsForKillingRobot()
    {
        points += _pointsForKill;
        updateScore();
    }

    private void updateScore()
    {
        _scoreText.text = (points / 1000 % 10).ToString() + (points / 100 % 10).ToString() +
                          (points / 10 % 10).ToString() +
                          (points % 10).ToString();
    }
    
    private void CheckHumanoidEscaped()
    {
        float minX = _maze.cellBounds.xMin;
        float minY = _maze.cellBounds.yMin;
        float maxX = _maze.cellBounds.xMax;
        float maxY = _maze.cellBounds.yMax;

        Vector3 humanoidPoisition = humanoid.transform.position;
        
        if (humanoidPoisition.y < minX) StartCoroutine(HumanoidEscaped(Vector2.down));
        else if (humanoidPoisition.y > maxX) StartCoroutine(HumanoidEscaped(Vector2.up));
        else if (humanoidPoisition.x < minY) StartCoroutine(HumanoidEscaped(Vector2.left));
        else if (humanoidPoisition.x > maxY) StartCoroutine(HumanoidEscaped(Vector2.right));
    }

    /**
     * Used to reset the game stats
     */
    public static void ResetGameStats()
    {
        level = STARTING_LEVEL;
        lives = STARTING_LIVES;
        points = STARTING_POINTS;
        EnterDirection = Vector2.zero;
    }

    /**
     * Gets the current robot difficulty level
     */
    public Robot GetRobotByDifficultyLevel()
    {
        int curDifficultyLevel = level % _difficultyIncreaseInterval == 0 ? difficultyLevel - 1 : difficultyLevel;
        curDifficultyLevel %= _robots.Length;
        return _robots[curDifficultyLevel];
    }
}
