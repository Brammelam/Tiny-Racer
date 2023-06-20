using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoresSO : ScriptableObject
{
    private static ScoresSO _instance;

    public static ScoresSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ScoresSO>("SO/ScoresSO");
            }
            return _instance;
        }
    }

    [SerializeField]
    private List<string> _names;
    [SerializeField]
    public List<int> _scores;
    [SerializeField]
    private List<int> _playerScores;
    [SerializeField]
    public int _levelIndex;

    public event Action<int> LevelIndexChanged;

    public List<string> Names
    {
        get { return _names; }
        set { _names = value; }
    }

    public List<int> Values
    {
        get { return _scores; }
        set { _scores = value; }
    }

    public List<int> PlayerValues
    {
        get { return _playerScores; }
        set
        {
            _playerScores = value;

        }
    }

    public int CurrentLevel
    {
        get { return _levelIndex; }
        set
        {
            _levelIndex = value;
            OnLevelIndexChanged();
        }
    }

    private void OnLevelIndexChanged()
    {
        LevelIndexChanged?.Invoke(_levelIndex);
    }
}