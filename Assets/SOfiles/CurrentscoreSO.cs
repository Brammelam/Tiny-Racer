using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CurrentscoreSO : ScriptableObject
{
    [SerializeField]
    private float _currentScore;

    public float CurrentScore
    {
        get { return _currentScore; }
        set { _currentScore = value; }
    }

}
