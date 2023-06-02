using UnityEngine;

[CreateAssetMenu]
public class CurrentscoreSO : ScriptableObject
{
    [SerializeField]
    private float _currentScore;
    [SerializeField]
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
