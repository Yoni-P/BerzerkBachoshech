using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Loads a newly loaded level
 */
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private Humanoid _humanoid;
     private void Start()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        if (GameManager.level % 5 == 0)
            _mazeGenerator.GenerateBossMaze(GameManager.EnterDirection);
        else
            _mazeGenerator.GenerateRandomMaze(GameManager.EnterDirection);
        
        _mazeGenerator.DrawMaze();
        Humanoid humanoid = Instantiate(_humanoid.gameObject, _mazeGenerator.HumanoidSpawnPoint(),
            Quaternion.identity).GetComponent<Humanoid>();
        yield return new WaitUntil(() => humanoid.finishedSpawning);
        _mazeGenerator.SpawnRobots();
    }
}
