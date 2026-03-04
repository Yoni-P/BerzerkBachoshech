using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;
using Random = UnityEngine.Random;

/**
 * Generates and draws mazes for the levels
 */
public class MazeGenerator : MonoBehaviour
{
    private const string ROBOT_MAKER = "Robot Maker";

    [SerializeField] private Tilemap _maze, _mazeFloor, _gates;
    [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;
    [SerializeField] private GameObject _robotMaker;
    [SerializeField] private Transform _mazeCenter;
    [SerializeField] private Tile _horizontalGate,
        _verticalGate,
        _horizontalInnerWall,
        _verticalInnerWall,
        _horizontalOuterWall,
        _verticalOuterWall,
        _floorTile;

    private int _width, _height, _verticalBorderWidth, _horizontalBorderWidth;
    private Dictionary<Border, Tile> _horizontalBorderTiles = new ();
    private Dictionary<Border, Tile> _verticalBorderTiles = new ();
    private MazeCell[][] _mazeCells;
    private static int _generatorSeed = 0;
    private List<GameObject> _createsObjects = new List<GameObject>();
    private GameObject _curHumanoid;
    private Vector3 _humanoidSpawnPoint;
    private Grid _cellGrid;
    private Robot _robot;
    
    private void Start()
    {
        _height = GameManager.Instance.MazeHeight;
        _width = GameManager.Instance.MazeWidth;
        _horizontalBorderWidth = GameManager.Instance.MazeCellWidth;
        _verticalBorderWidth = GameManager.Instance.MazeCellHeight;
        _cellGrid = GameManager.Instance.MazeCellGrid;

        _horizontalBorderWidth--;
        _verticalBorderWidth--;
        _mazeCells = new MazeCell[_height][];
        ResetBorders();

        _robot = GameManager.Instance.GetRobotByDifficultyLevel();

        _horizontalBorderTiles.Add(Border.INNER_WALL, _horizontalInnerWall);
        _horizontalBorderTiles.Add(Border.GATE, _horizontalGate);
        _horizontalBorderTiles.Add(Border.OUTER_WALL, _horizontalOuterWall);

        _verticalBorderTiles.Add(Border.INNER_WALL, _verticalInnerWall);
        _verticalBorderTiles.Add(Border.GATE, _verticalGate);
        _verticalBorderTiles.Add(Border.OUTER_WALL, _verticalOuterWall);

        RobotMaker.onTimerDone += RemoveGates;
    }
    
    /**
     * Generates a random maze
     */
    public void GenerateRandomMaze(Vector2 enterDirection)
    {
        _generatorSeed += 1;
        ResetBorders();
        InitializeGates(enterDirection);
        DefineSpawnPoint(enterDirection);
        InitializeOuterWalls();
        RandomlyGenerateInnerWalls();
    }
    
    private void ResetBorders()
    {
        System.Random random = new System.Random(_generatorSeed);
        for (int i = 0; i < _height; i++)
        {
            _mazeCells[i] = new MazeCell[_width];
            for (int j = 0; j < _width; j++)
            {
                _mazeCells[i][j] = new MazeCell(new Vector3Int(i, j));
            }
        }
    }
    private void InitializeGates(Vector2 enterDirection)
    {
        _mazeCells[0][_width / 2].upperBorder = enterDirection == Vector2.up || enterDirection == Vector2.one
            ? Border.GATE : Border.EXIT;
        _mazeCells[_height - 1][_width / 2].lowerBorder = enterDirection == Vector2.down || enterDirection == Vector2.one
            ? Border.GATE : Border.EXIT;
        _mazeCells[_height / 2][0].leftBorder = enterDirection == Vector2.left || enterDirection == Vector2.one
            ? Border.GATE : Border.EXIT;
        _mazeCells[_height / 2][_width - 1].rightBorder = enterDirection == Vector2.right || enterDirection == Vector2.one
            ? Border.GATE : Border.EXIT;
    }
    private void DefineSpawnPoint(Vector2 enterDirection)
    {
        int spawnX = 0, spawnY = 0;
        if (enterDirection == Vector2.left || enterDirection == Vector2.zero)
        {
            spawnX = _height / 2;
            spawnY = 0;
        }
        else if (enterDirection == Vector2.up)
        {
            spawnX = 0;
            spawnY = _width / 2;
        }
        else if (enterDirection == Vector2.right)
        {
            spawnX = _height / 2;
            spawnY = _width - 1;
        }
        else if (enterDirection == Vector2.down)
        {
            spawnX = _height - 1;
            spawnY = _width / 2;
        }

        _mazeCells[spawnX][spawnY].cellType = "Spawn";
        _humanoidSpawnPoint = _cellGrid.GetCellCenterWorld(new Vector3Int(-spawnX, spawnY));
    }
    private void InitializeOuterWalls()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (i == 0 && _mazeCells[i][j].upperBorder == Border.UNDEFINED)
                    _mazeCells[i][j].upperBorder = Border.OUTER_WALL;
                if (i == _height - 1 && _mazeCells[i][j].lowerBorder == Border.UNDEFINED)
                    _mazeCells[i][j].lowerBorder = Border.OUTER_WALL;
                if (j == 0 && _mazeCells[i][j].leftBorder == Border.UNDEFINED)
                    _mazeCells[i][j].leftBorder = Border.OUTER_WALL;
                if (j == _width - 1 && _mazeCells[i][j].rightBorder == Border.UNDEFINED)
                    _mazeCells[i][j].rightBorder = Border.OUTER_WALL;
            }
        }
    }
    private void RandomlyGenerateInnerWalls()
    {
        System.Random random = new System.Random(_generatorSeed);
        HashSet<Vector2Int> visitedUnits = new HashSet<Vector2Int>();
        int x = random.Next(_height);
        int y = random.Next(_width);

        TravelMaze(new Vector2Int(x, y), ref _mazeCells[x][y], ref visitedUnits, random);

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                ref MazeCell curMazeUnit = ref _mazeCells[i][j];
                if (curMazeUnit.leftBorder == Border.UNDEFINED) curMazeUnit.leftBorder = Border.INNER_WALL;
                if (curMazeUnit.rightBorder == Border.UNDEFINED) curMazeUnit.rightBorder = Border.INNER_WALL;
                if (curMazeUnit.upperBorder == Border.UNDEFINED) curMazeUnit.upperBorder = Border.INNER_WALL;
                if (curMazeUnit.lowerBorder == Border.UNDEFINED) curMazeUnit.lowerBorder = Border.INNER_WALL;
            }
        }
    }
    private void TravelMaze(Vector2Int unitCoords, ref MazeCell curMazeCell, ref HashSet<Vector2Int> visitedUnits,
        System.Random random)
    {
        visitedUnits.Add(unitCoords);
        
        Vector2Int[] neighbourDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};
        HashSet<Vector2> chekedNeighbours = new HashSet<Vector2>();
        while (visitedUnits.Count < _height * _width)
        {
            if (chekedNeighbours.Count() == 4 || !curMazeCell.borders.Values.Contains(Border.UNDEFINED)) return;

            int i;
            do
            {
                i = random.Next(neighbourDirections.Length);
                chekedNeighbours.Add(neighbourDirections[i]);
            }
            while (curMazeCell.borders[neighbourDirections[i]] != Border.UNDEFINED) ;

            Vector2Int neighbourDirection = neighbourDirections[i];
            Vector2Int nextCellCoords = new Vector2Int(unitCoords.x - (int)neighbourDirection.y,
                unitCoords.y + (int)neighbourDirection.x);
            bool canEnterNextUnit = !visitedUnits.Contains(nextCellCoords);
            
            if (canEnterNextUnit)
            {
                curMazeCell.borders[neighbourDirection] = Border.OPENING;
                _mazeCells[nextCellCoords.x][nextCellCoords.y].borders[neighbourDirection*-1] = Border.OPENING;
                TravelMaze(nextCellCoords, ref _mazeCells[nextCellCoords.x][nextCellCoords.y], ref visitedUnits,
                    random);
            }
        }
    }

    /**
     * Generates a special maze for the boss levels
     */
    public void GenerateBossMaze(Vector2 enterDirection)
    {
        ResetBorders();
        InitializeGates(Vector2.one);
        DefineSpawnPoint(enterDirection);
        InitializeOuterWalls();
        GenerateBossMazeInnerWalls();
    }
    private void GenerateBossMazeInnerWalls()
    {
        ref MazeCell centerCell = ref _mazeCells[_height / 2][_width / 2];
        centerCell.cellType = ROBOT_MAKER;

        centerCell.upperBorder = Border.INNER_WALL;
        centerCell.rightBorder = Border.INNER_WALL;
        centerCell.lowerBorder = Border.INNER_WALL;
        centerCell.leftBorder = Border.INNER_WALL;
        
        foreach (var border in centerCell.borders.Keys)
        {
            _mazeCells[centerCell.cellCorrdinates.x - border.y][centerCell.cellCorrdinates.y + border.x]
                .borders[-border] = Border.INNER_WALL;
        }

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (_mazeCells[i][j].upperBorder == Border.UNDEFINED) _mazeCells[i][j].upperBorder = Border.OPENING;
                if (_mazeCells[i][j].rightBorder == Border.UNDEFINED) _mazeCells[i][j].rightBorder = Border.OPENING;
                if (_mazeCells[i][j].lowerBorder == Border.UNDEFINED) _mazeCells[i][j].lowerBorder = Border.OPENING;
                if (_mazeCells[i][j].leftBorder == Border.UNDEFINED) _mazeCells[i][j].leftBorder = Border.OPENING;
            }
        }
        
        Instantiate(_robotMaker, centerCell.MazeCellCenterToWorld(), Quaternion.identity);
    }
    
    /**
     * Spawns robots in the maze
     */
    public void SpawnRobots()
    {
        System.Random random = new System.Random(_generatorSeed);
        _createsObjects = new List<GameObject>();
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (_mazeCells[i][j].cellType == null)
                {
                    BoxCollider2D robotCollider = _robot.GetComponent<BoxCollider2D>();
                    int numOfRobots = random.Next(2);
                    for (int k = 0; k < numOfRobots; k++)
                    {
                        Vector3 cellCenter = _cellGrid.GetCellCenterWorld(new Vector3Int(-i, j));
                        Vector3 cellExtents = new Vector3(_cellGrid.cellSize.x / 4, -_cellGrid.cellSize.y / 4);
                        Vector3 topLeftCorner = cellCenter - cellExtents;
                        Vector3 bottomRightCorner = cellCenter + cellExtents;

                        int x = random.Next((int)topLeftCorner.x, (int)bottomRightCorner.x);
                        int y = random.Next((int)bottomRightCorner.y, (int)topLeftCorner.y);
                        while (Physics.BoxCast(new Vector3(x, y), robotCollider.bounds.extents, Vector3.zero))
                        {
                            x = random.Next((int)topLeftCorner.x, (int)bottomRightCorner.x);
                            y = random.Next((int)bottomRightCorner.y, (int)topLeftCorner.y);
                        }

                        _createsObjects.Add(Instantiate(_robot.gameObject, new Vector3(x, y),
                            Quaternion.identity));
                    }
                }
            }
        }
    }
    
    private void DrawFloor()
    {
        for (int i = 0; i < _height * _verticalBorderWidth + 1; i++)
        {
            for (int j = 0; j < _width * _horizontalBorderWidth + 1; j++)
            {
                _mazeFloor.SetTile(new Vector3Int(-i, j), _floorTile);
            }
        }
    }
    
    /**
     * Draws the generated maze
     */
    public void DrawMaze()
    {
        DrawFloor();
        _maze.ClearAllTiles();
        DrawInnerWalls();
        DrawOuterWalls();
        _maze.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
        Vector2 mazeCenterCoords = new Vector2(_maze.cellBounds.center.y, _maze.cellBounds.center.x);
        _mazeCenter.position = mazeCenterCoords;
        int pixelsPerUnit = _pixelPerfectCamera.assetsPPU;
        _pixelPerfectCamera.refResolutionX = (_width * _horizontalBorderWidth + 1 + 4) * pixelsPerUnit;
        _pixelPerfectCamera.refResolutionY = (_height * _verticalBorderWidth + 1 + 4) * pixelsPerUnit;
    }

    private void DrawInnerWalls()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                int x = -(i * _verticalBorderWidth);
                int y = j * _horizontalBorderWidth;

                if (_mazeCells[i][j].upperBorder == Border.INNER_WALL)
                    DrawHorizontalBorder(x, y, _mazeCells[i][j].upperBorder);

                if (_mazeCells[i][j].leftBorder == Border.INNER_WALL)
                    DrawVerticalBorder(x, y, _mazeCells[i][j].leftBorder);
            }
        }
    }

    private void DrawOuterWalls()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                int x = -(i * _verticalBorderWidth);
                int y = j * _horizontalBorderWidth;

                if (_mazeCells[i][j].upperBorder == Border.OUTER_WALL || _mazeCells[i][j].upperBorder == Border.GATE)
                    DrawHorizontalBorder(x, y, _mazeCells[i][j].upperBorder);

                if (_mazeCells[i][j].leftBorder == Border.OUTER_WALL || _mazeCells[i][j].leftBorder == Border.GATE)
                    DrawVerticalBorder(x, y, _mazeCells[i][j].leftBorder);

                if (_mazeCells[i][j].rightBorder == Border.OUTER_WALL || _mazeCells[i][j].rightBorder == Border.GATE)
                    DrawVerticalBorder(x, y + _horizontalBorderWidth, _mazeCells[i][j].rightBorder);

                if (_mazeCells[i][j].lowerBorder == Border.OUTER_WALL || _mazeCells[i][j].lowerBorder == Border.GATE)
                    DrawHorizontalBorder(x - _verticalBorderWidth, y, _mazeCells[i][j].lowerBorder);
            }
        }
    }

    private void DrawVerticalBorder(int x, int y, Border borderType)
    {
        if (borderType == Border.GATE)
            _maze.SetTile(new Vector3Int(x++, y),
                _verticalBorderTiles[Border.OUTER_WALL]);
        Tilemap tileMapToDrawOn = borderType == Border.GATE ? _gates : _maze;
        for (int i = x; i > x - _verticalBorderWidth - 1; i--)
        {
            tileMapToDrawOn.SetTile(new Vector3Int(i, y), _verticalBorderTiles[borderType]);
        }
    }

    private void DrawHorizontalBorder(int x, int y, Border borderType)
    {
        if (borderType == Border.GATE)
            _maze.SetTile(new Vector3Int(x, y++),
                _horizontalBorderTiles[Border.OUTER_WALL]);
        Tilemap tileMapToDrawOn = borderType == Border.GATE ? _gates : _maze;
        for (int i = y; i < y + _horizontalBorderWidth + 1; i++)
        {
            tileMapToDrawOn.SetTile(new Vector3Int(x, i), _horizontalBorderTiles[borderType]);
        }
    }

    public Vector3 HumanoidSpawnPoint()
    {
        return _humanoidSpawnPoint;
    }

    private void RemoveGates()
    {
        StartCoroutine(GateRemovingAnimation());
    }

    private IEnumerator GateRemovingAnimation()
    {
        TilemapRenderer gatesRenderer = _gates.GetComponent<TilemapRenderer>();
        for (int i = 0; i < 3; i++)
        {
            gatesRenderer.enabled = false;
            yield return new WaitForSeconds(0.5f);
            gatesRenderer.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(_gates.gameObject);
    }

    private void OnDestroy()
    {
        RobotMaker.onTimerDone -= RemoveGates;
    }
}
