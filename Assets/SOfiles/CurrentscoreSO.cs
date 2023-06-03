using UnityEngine;
using System;

[CreateAssetMenu]
public class CurrentscoreSO : ScriptableObject
{
    [SerializeField]
    private float _currentScore;
    [SerializeField]
    private float _currentPlayerScore;
    [SerializeField]
    private string _playerName;

    public event Action<float> OnScoreChanged;
    public event Action<float> OnPlayerScoreChanged;


    public float CurrentScore
    {
        get { return _currentScore; }
        set 
        {
            _currentScore = value;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }

    public float CurrentPlayerScore
    {
        get { return _currentPlayerScore; }
        set 
        { 
            _currentPlayerScore = value;
            OnPlayerScoreChanged?.Invoke(_currentPlayerScore);
        }
    }

    public string CurrentPlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }
}
