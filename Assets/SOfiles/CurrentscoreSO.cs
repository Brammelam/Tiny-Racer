using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CurrentscoreSO : ScriptableObject
{
    [SerializeField]
    private float _currentScore;
    private float _currentPlayerScore;

    public float CurrentScore
    {
        get { return _currentScore; }
        set { _currentScore = value; }
    }

    public float CurrentPlayerScore
    {
        get { return _currentPlayerScore; }
        set { _currentPlayerScore = value; }
    }

}
