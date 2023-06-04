using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoresSO : ScriptableObject
{
    [SerializeField]
    private List<string> _names;
    [SerializeField]
    public List<string> _scores;
    [SerializeField]
    private List<string> _playerScores;
    [SerializeField]
    public int _levelIndex;

    public event Action<int> LevelIndexChanged;

    public List<string> Names
    {
        get { return _names; }
        set { _names = value; }
    }

    public List<string> Values
    {
        get { return _scores; }
        set { _scores = value; }
    }

    public List<string> PlayerValues
    {
        get { return _playerScores; }
        set { _playerScores = value; }
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